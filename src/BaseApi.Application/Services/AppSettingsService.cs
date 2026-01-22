using BaseApi.Application.DTOs;
using BaseApi.Application.Interfaces;

namespace BaseApi.Application.Services;

public class AppSettingsService : IAppSettingsService
{
    private readonly IAppSettingsRepository _repository;

    public AppSettingsService(IAppSettingsRepository repository)
    {
        _repository = repository;
    }

    public async Task<AppSettingsResponse?> GetSettingsAsync()
    {
        var settings = await _repository.GetAsync();

        if (settings == null)
            return null;

        return new AppSettingsResponse
        {
            Id = settings.Id,
            BrandName = settings.BrandName,
            LogoUrl = settings.LogoUrl,
            PrimaryColor = settings.PrimaryColor,
            SecondaryColor = settings.SecondaryColor,
            AccentColor = settings.AccentColor,
            UpdatedAt = settings.UpdatedAt
        };
    }

    public async Task<AppSettingsResponse> UpdateSettingsAsync(UpdateAppSettingsRequest request)
    {
        var settings = await _repository.GetAsync();

        if (settings == null)
            throw new InvalidOperationException("Configurações não encontradas. Execute o seed do banco de dados.");

        // Atualizar propriedades
        settings.BrandName = request.BrandName;
        settings.LogoUrl = request.LogoUrl;
        settings.PrimaryColor = request.PrimaryColor;
        settings.SecondaryColor = request.SecondaryColor;
        settings.AccentColor = request.AccentColor;

        var updated = await _repository.UpdateAsync(settings);

        return new AppSettingsResponse
        {
            Id = updated.Id,
            BrandName = updated.BrandName,
            LogoUrl = updated.LogoUrl,
            PrimaryColor = updated.PrimaryColor,
            SecondaryColor = updated.SecondaryColor,
            AccentColor = updated.AccentColor,
            UpdatedAt = updated.UpdatedAt
        };
    }
}
