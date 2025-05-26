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
using ApiGateway.Proxying;
using ApiGateway.Logging.models;
using ApiGateway.Logging.abstracts;
using ApiGateway.Logging.services;
using ApiGateway.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ApiGateway.Proxying.policies;


namespace ApiGateway;

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

            services.Configure<JwtOptions>(config.GetSection("Jwt"));
            services.Configure<ProxyLogOptions>(config.GetSection("ProxyLog"));
            services.Configure<MongoSettings>(config.GetSection("Mongo"));

            services.AddSingleton<TokenValidationParameters>(sp =>
            {
                var jwt = sp.GetRequiredService<IOptions<JwtOptions>>().Value;

                return new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret))
                };
            });
            
            
            services.AddSingleton<IRequestFilterValueExtractor, DefaultRequestFilterValueExtractor>();
            services.AddSingleton<IRateLimitingStrategy, FixedWindowStrategy>();
            services.AddSingleton<IRateLimitingStrategySelector, RateLimitingStrategySelector>();
            services.AddSingleton<IKeyBuilder, DefaultKeyBuilder>();
        
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                return ConnectionMultiplexer.Connect("localhost:6380"); 
            });
        
            services.AddSingleton<IRateLimitStore, RedisRateLimitStore>();
            services.AddSingleton<IRateLimitConfigProvider>(sp =>
            {
                var extractor = sp.GetRequiredService<IRequestFilterValueExtractor>();
                var redis = sp.GetRequiredService<IConnectionMultiplexer>();
                return new RedisRateLimitConfigProvider(redis, extractor);
            });  
        
            services.AddSingleton<ILogWriter, MongoLogWriter>();
            services.AddSingleton<IMasterLogService, MasterLogService>();

            services.AddHttpContextAccessor();
            services.AddSingleton<PolicyFactory>();
            services.AddHttpClient("UserApiClient", client =>
            {
                client.BaseAddress = new Uri("http://localhost:5276");
            })
            .AddPolicyHandler((sp, req) =>
            {
                return sp.GetRequiredService<PolicyFactory>().CreatePolicy();
            });

            services.AddScoped<IProxyService, ProxyService>();



            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<JwtAuthMiddleware>();
            app.UseMiddleware<RateLimitingMiddleware>();
            app.UseAuthorization();
            app.MapControllers();
            app.UseMiddleware<ExceptionMiddleware>();
            app.Run();


    }
}
