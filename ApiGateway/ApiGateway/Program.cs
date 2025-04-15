using ApiGateway.Middleware;
using ApiGateway.RateLimiting.strategies;
using ApiGateway.Redis;
using Serilog;
using ApiGateway.ConfigLoader;
using ApiGateway.RateLimiting.core;
using ApiGateway.RateLimiting.Selector;
using ApiGateway.ConfigLoader.extractFilterValues;
using ApiGateway.ConfigLoader.keyBuild;
using StackExchange.Redis;
using ApiGateway.ConfigLoader.providers.redis;
using ApiGateway.Logging;

namespace ApiGateway;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File("logs/request.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try 
        {
            Log.Information("Starting up API Gateway");

            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            builder.Services.AddSingleton<IRequestFilterValueExtractor, DefaultRequestFilterValueExtractor>();

            builder.Services.AddSingleton<IRateLimitConfigProvider>(sp =>
            {
                var extractor = sp.GetRequiredService<IRequestFilterValueExtractor>();
                var redis = sp.GetRequiredService<IConnectionMultiplexer>();
                return new RedisRateLimitConfigProvider(redis, extractor);
            });  
            
            builder.Services.AddSingleton<IRateLimitingStrategy, FixedWindowStrategy>();
            builder.Services.AddSingleton<IRateLimitingStrategySelector, RateLimitingStrategySelector>();
            builder.Services.AddSingleton<IKeyBuilder, DefaultKeyBuilder>();

            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                return ConnectionMultiplexer.Connect("localhost:6380"); 
            });

            builder.Services.AddSingleton<IRateLimitStore, RedisRateLimitStore>();
            builder.Services.AddSingleton<ILogService, MongoLogService>();


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<RateLimitingMiddleware>();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application failed to start.");
        }
        finally
        {
            Log.CloseAndFlush(); 
        }





    }
}
