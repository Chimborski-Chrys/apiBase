using BaseApi.Application.Interfaces;
using BaseApi.Domain.Entities;
using BaseApi.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace BaseApi.Infra.Repositories;

public class AppSettingsRepository : IAppSettingsRepository
{
    private readonly ApplicationDbContext _context;

    public AppSettingsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AppSettings?> GetAsync()
    {
        return await _context.AppSettings.FirstOrDefaultAsync();
    }

    public async Task<AppSettings> UpdateAsync(AppSettings settings)
    {
        _context.AppSettings.Update(settings);
        await _context.SaveChangesAsync();
        return settings;
    }
}
