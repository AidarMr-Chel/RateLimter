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

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddScoped<ILogService, LogService>();
        builder.Services.AddScoped<ILogRepository, LogRepository>();
        builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("Mongo"));

        builder.Services.AddScoped<IPolicyRepository, PolicyRepository>();
        builder.Services.AddScoped<IPolicyService, PolicyService>();

        builder.Services.AddScoped<FilterValidator>();
        builder.Services.AddScoped<RuleValidator>();


        builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            return ConnectionMultiplexer.Connect("localhost:6380");
        });

        var app = builder.Build();
        
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
