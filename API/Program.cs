using System.Text.Json.Serialization;
using API;
using API.Extensions;
using Application;
using HealthChecks.UI.Client;
using Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        
        builder.Configuration.AddEnvironmentVariables();
        
        builder.Services.AddSwaggerGenWithAuth(); 
        
        builder.Services
            .AddApplication()
            .AddPresentation()
            .AddInfrastructure(builder.Configuration);
        
        builder.Services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        
        var app = builder.Build();

        app.UseSwaggerWithUi();  

        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.UseCors("AllowLocalAndProdFE");
        app.UseRequestContextLogging();
        app.UseStaticFiles();
        app.UseExceptionHandler();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.ApplyMigrations();  // chạy EF Core migration khi khởi động
        app.MapControllers();

        app.Run();
    }
}