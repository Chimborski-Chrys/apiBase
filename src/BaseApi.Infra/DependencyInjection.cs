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
            options.UseNpgsql(connectionString));

        // Registrar repositórios
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
