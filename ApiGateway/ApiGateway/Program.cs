using ApiGateway.Middleware;
using ApiGateway.RateLimiting.strategies;
using ApiGateway.Redis;
using ApiGateway.RateLimiting.Selector;
using StackExchange.Redis;
using ApiGateway.Proxying;
using ApiGateway.Logging.models;
using ApiGateway.Logging.abstracts;
using ApiGateway.Logging.services;
using ApiGateway.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ApiGateway.Proxying.policies;
using ApiGateway.RateLimiting.keyBuild;
using ApiGateway.RateLimiting.extractFilterValues;
using ApiGateway.RateLimiting.configPolicy.abstracts;
using ApiGateway.RateLimiting.configPolicy.matching;
using ApiGateway.RateLimiting.configPolicy.storage;
using ApiGateway.RateLimiting.core.abstracts;


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
            services.Configure<BackendPathOptions>(config.GetSection("BackendPath"));

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

            services.AddScoped<RateLimitingMiddleware>();
            services.AddScoped<IRequestFilterValueExtractor, DefaultRequestFilterValueExtractor>();
            services.AddScoped<IKeyBuilder, DefaultKeyBuilder>();
            services.AddScoped<IRateLimitingStrategy, FixedWindowStrategy>();
            services.AddScoped<IRateLimitingStrategy, SlidingWindowStrategy>();
            services.AddScoped<IRateLimitingStrategySelector, RateLimitingStrategySelector>();

            services.AddSingleton<IConnectionMultiplexer>(sp =>
                {
                    return ConnectionMultiplexer.Connect("localhost:6380"); 
                });
        
            services.AddSingleton<IRateLimitStore, RedisRateLimitStore>();
            services.AddScoped<IRateLimitConfigStore, RedisRateLimitConfigStore>();
            services.AddScoped<IRateLimitRuleMatcher, DefaultRuleMatcher>();

            services.AddSingleton<ILogWriter, MongoLogWriter>();
            services.AddSingleton<IMasterLogService, MasterLogService>();

            services.AddHttpContextAccessor();
            services.AddSingleton<PolicyFactory>();
            services.AddHttpClient("UserApiClient", client =>
            {
                client.BaseAddress = new Uri(config.GetSection("BackendPath").GetValue<string>("Path"));
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
            app.Use((RequestDelegate next) =>
            {
                return async context =>
                {
                    var middleware = context.RequestServices.GetRequiredService<RateLimitingMiddleware>();
                    await middleware.InvokeAsync(context, next);
                };
            });
            app.UseAuthorization();
            app.MapControllers();
            app.UseMiddleware<ExceptionMiddleware>();
            app.Run();


    }
}
