using BaseApi.Application.DTOs;

namespace BaseApi.Application.Interfaces;

public interface IAppSettingsService
{
    Task<AppSettingsResponse?> GetSettingsAsync();
    Task<AppSettingsResponse> UpdateSettingsAsync(UpdateAppSettingsRequest request);
}
