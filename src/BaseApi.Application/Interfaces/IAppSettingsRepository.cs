using BaseApi.Domain.Entities;

namespace BaseApi.Application.Interfaces;

public interface IAppSettingsRepository
{
    Task<AppSettings?> GetAsync();
    Task<AppSettings> UpdateAsync(AppSettings settings);
}
