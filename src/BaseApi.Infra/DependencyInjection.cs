using BaseApi.Application.Interfaces;
using BaseApi.Infra.Data;
using BaseApi.Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BaseApi.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found. " +
                "Please configure User Secrets. Run: dotnet user-secrets set \"ConnectionStrings:DefaultConnection\" \"your-connection-string\"");
        }

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                // Aumentar timeout de comando para 60 segundos (padrão é 30)
                npgsqlOptions.CommandTimeout(60);
                // Tentar reconectar automaticamente em caso de falha
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorCodesToAdd: null);
            }));

        // Registrar repositórios
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAppSettingsRepository, AppSettingsRepository>();

        return services;
    }
}
