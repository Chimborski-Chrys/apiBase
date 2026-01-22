using BaseApi.Application.Interfaces;
using BaseApi.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BaseApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAppSettingsService, AppSettingsService>();

        return services;
    }
}
