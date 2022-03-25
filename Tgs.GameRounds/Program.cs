using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using Tgs.GameRounds;

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
            var nodeIp = builder.Configuration.GetValue<string>("NODE_IP");
            fluentOptions.Host = nodeIp;
            fluentOptions.Tag = builder.Configuration.GetValue<string>("OTEL_SERVICE_NAME");
            fluentOptions.UseMessagePack();
        });
    });

// Add services to the container.

builder.Services
    .AddRouting(options => options.LowercaseUrls = true)
    .AddControllers()
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
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tgs.GameRounds", Version = "v1" });
});

builder.Services
    .AddRouting(options => options.LowercaseUrls = true)
    .AddControllers(configure =>
    {
        _ = configure.Filters.Add<ExceptionFilter>();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services
    .AddGameRounds(
    (options, sp) =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        options.ConnectionString = configuration.GetConnectionString("Mongo");
    },
    (options, sp) =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        options.ConnectionString = configuration.GetConnectionString("Redis");
    })
    .AddGameRoundsCore(
    configuration =>
    {
        _ = configuration
           .AddGameRoundTgsRepositories((sp, dbBuilder) =>
           {
               var configuration = sp.GetRequiredService<IConfiguration>();
               _ = dbBuilder.UseNpgsql(configuration.GetConnectionString("Tgs"),
                   serverOption =>
                   {
                       _ = serverOption.SetPostgresVersion(12, 5);
                       _ = serverOption.CommandTimeout(180);
                   });
           },
           sp => sp.GetRequiredService<RedisConnectionManager>().Database);
    },
    (sp) => sp.GetRequiredService<RedisConnectionManager>().Database)
    .AddGameRoundRepositories((sp) => sp.GetRequiredService<MongoDBConnectionManager>().Database)
    .AddGameRoundBetRepositories(
    (sp, dbBuilder) =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        _ = dbBuilder.UseNpgsql(configuration.GetConnectionString("Tgs"),
            serverOption =>
            {
                _ = serverOption.SetPostgresVersion(12, 5);
                _ = serverOption.CommandTimeout(180);
            });
    });

builder.Services
    .AddMessageQueue()
    .AddNatsMessageQueue(natsConfig =>
    {
        _ = natsConfig
            .ConfigQueueOptions((options, sp) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();

                options.Url = config.GetConnectionString("Nats");
            });
    })
    .AddNatsGlobPatternExchange("*");

builder.Services
     .AddSingleton<IConfigureOptions<OtlpExporterOptions>>(
        sp => new ConfigureNamedOptions<OtlpExporterOptions>(
            Microsoft.Extensions.Options.Options.DefaultName,
            exporterOptions =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var nodeIp = config.GetValue<string>("NODE_IP");

                if (!string.IsNullOrWhiteSpace(nodeIp))
                    exporterOptions.Endpoint = new Uri($"http://{nodeIp}:4317");

                config.GetSection("Opentelemetry:Tracing").Bind(exporterOptions);
            }))
    .AddOpenTelemetryTracing(traceBuilder => traceBuilder
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddEnvironmentVariableDetector())
        .AddSource("Tgs.*")
        .AddSource("TGS.*")
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddProfileViewExporter()
        .AddOtlpExporter())
    .AddProfileViewer()
    .Configure<AspNetCoreInstrumentationOptions>(options =>
    {
        options.Filter = httpContext =>
        {
            return !httpContext.Request.Path.HasValue
                || !httpContext.Request.Path.StartsWithSegments("/swagger")
                && !httpContext.Request.Path.StartsWithSegments("/healthz")
                && !httpContext.Request.Path.StartsWithSegments("/profiler/")
                && !httpContext.Request.Path.StartsWithSegments("/metrics")
                && !new[] { "html", "ico", "png", "jpg" }.Any(
                    ext => httpContext.Request.Path.Value.EndsWith($".{ext}"));
        };
    });

builder.Services
    .AddInterceptionService()
    .AddServiceUtiltiyInterceptors();

builder.Services
    .AddHealthChecks()
    .ForwardToPrometheus();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/healthz");

app.MapMetrics();

app.MapProfileViewer();

app.Run();
