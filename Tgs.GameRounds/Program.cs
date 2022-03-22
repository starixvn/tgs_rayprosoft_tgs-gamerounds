using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Tgs.GameRounds;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
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
        configure.Filters.Add<ExceptionFilter>();
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
        options.ConnectionString = configuration.GetConnectionString("MongoDBConnectionString");
    },
    (options, sp) =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        options.ConnectionString = configuration.GetConnectionString("RedisConfigurationString");
    })
    .AddGameRoundsCore((sp) => sp.GetRequiredService<RedisConnectionManager>().Database)
    .AddGameRoundRepositories((sp) => sp.GetRequiredService<MongoDBConnectionManager>().Database)
    .AddGameRoundTgsRepositories(
    (sp, dbBuilder) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();
        dbBuilder.UseNpgsql(config.GetConnectionString("Tgs"),
            serverOption =>
            {
                serverOption.SetPostgresVersion(12, 5);
                serverOption.CommandTimeout(180);
            });
    })
    .AddGameRoundBetRepositories(
    (sp, dbBuilder) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();
        dbBuilder.UseNpgsql(config.GetConnectionString("Tgs"),
            serverOption =>
            {
                serverOption.SetPostgresVersion(12, 5);
                serverOption.CommandTimeout(180);
            });
    });

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

app.Run();
