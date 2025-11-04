using API.Infrastructure;
using API.Services;

namespace API;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddControllers();

        services.AddScoped<IVehicleImageStorage, VehicleImageStorage>();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddCors(options =>
        {
            options.AddPolicy("AllowLocalAndProdFE", policy =>
                policy.WithOrigins("http://localhost:5173", "https://corvus-fe.vercel.app")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .SetIsOriginAllowedToAllowWildcardSubdomains());
        });
        return services;
    }
}
