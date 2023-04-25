using System.Net.Mime;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("/config/appsettings.json", true)
    .AddEnvironmentVariables();

builder.Logging
    .AddOpenTelemetry(options =>
    {
        options.IncludeFormattedMessage = true;
        options.ParseStateValues = true;
        _ = options.AddFluentdForwardExporter(fluentOptions =>
        {
            var nodeIp = builder.Configuration.GetValue<string>("NODE_IP")!;
            fluentOptions.Host = nodeIp;
            fluentOptions.Tag = builder.Configuration.GetValue<string>("OTEL_SERVICE_NAME")!;
            fluentOptions.UseMessagePack();
        });
    });

// Add services to the container.

builder.Services
    .AddRouting(options => options.LowercaseUrls = true)
    .AddControllers(options =>
    {
        options.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
        options.Filters.Add(new ConsumesAttribute(MediaTypeNames.Application.Json));
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFileNames = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.xml");
    foreach (var xmlFileName in xmlFileNames)
    {
        c.IncludeXmlComments(xmlFileName);
    }
});

builder.Services
    .AddGameRounds(
    (options, sp) =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        options.ConnectionString = configuration.GetConnectionString("Redis")!;
    },
    (sp, dbBuilder) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();
        _ = dbBuilder.UseNpgsql(config.GetConnectionString("Tgs"));
    },
    (sp, dbBuilder) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();
        _ = dbBuilder.UseNpgsql(config.GetConnectionString("Tgs"));
    },
    (options, sp) =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        configuration.GetSection("GameLocked").Bind(options);
    });

builder.Services
    .AddOpenTelemetry()
    .WithTracing(builder => builder
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddEnvironmentVariableDetector())
        .AddSource("Tgs.*")
        .AddSource("TGS.*")
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddGrpcClientInstrumentation()
        .AddGrpcCoreInstrumentation()
        .AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true)
        .AddOtlpExporter())
    .WithMetrics(meterBuiler => meterBuiler
    .SetResourceBuilder(ResourceBuilder.CreateDefault()
    .AddEnvironmentVariableDetector())
    .AddRuntimeInstrumentation()
    .AddAspNetCoreInstrumentation()
    .AddOtlpExporter())
    .Services
    .Configure<AspNetCoreInstrumentationOptions>(options => options.Filter =
        httpContext => !httpContext.Request.Path.HasValue
            || !httpContext.Request.Path.StartsWithSegments("/swagger")
            && !httpContext.Request.Path.StartsWithSegments("/healthz")
            && !httpContext.Request.Path.StartsWithSegments("/profiler/")
            && !httpContext.Request.Path.StartsWithSegments("/hangfire")
            && !httpContext.Request.Path.StartsWithSegments("/metrics")
            && !new[] { "html", "ico", "png", "jpg" }.Any(
                ext => httpContext.Request.Path.Value.EndsWith($".{ext}")))
    .AddInterceptionService()
    .AddServiceUtiltiyInterceptors();

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
_ = app.UseSwagger();
_ = app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/healthz");

_ = app.UseRegister();

app.Run();
