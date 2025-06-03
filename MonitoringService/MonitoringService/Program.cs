using MonitoringService.Repositories;
using MonitoringService.Repositories.Abstracts;
using MonitoringService.Services.Absrtacts;
using MonitoringService.Services;
using MonitoringService.Models.loging;
using StackExchange.Redis;
using ApiGateway.RateLimiting.configPolicy.validation;

namespace MonitoringService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var config = builder.Configuration;
        var services = builder.Services;


        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddScoped<ILogService, LogService>();
        services.AddScoped<ILogRepository, LogRepository>();
        services.Configure<MongoSettings>(config.GetSection("Mongo"));

        services.AddScoped<IPolicyRepository, PolicyRepository>();
        services.AddScoped<IPolicyService, PolicyService>();
        services.AddScoped<IRpsMetricsRepository, RpsMetricsRepository>();
        services.AddSingleton<ISystemEventLogRepository, SystemEventLogRepository>();

        services.AddSingleton<ISystemEventLogRepository, SystemEventLogRepository>();
        services.AddScoped<ISystemEventLogService, SystemEventLogService>();

        services.AddScoped<FilterValidator>();
        services.AddScoped<RuleValidator>();
        services.AddScoped<IMetricService, MetricsService>();

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });



        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            return ConnectionMultiplexer.Connect("localhost:6379");
        });

        var app = builder.Build();
        app.UseCors();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
