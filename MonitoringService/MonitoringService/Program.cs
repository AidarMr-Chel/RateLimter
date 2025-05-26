using MonitoringService.Repositories;
using MonitoringService.Repositories.Abstracts;
using MonitoringService.Services.Absrtacts;
using MonitoringService.Services;
using MonitoringService.Models.loging;

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
