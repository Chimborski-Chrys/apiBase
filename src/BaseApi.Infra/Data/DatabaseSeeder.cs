using BaseApi.Domain.Entities;
using BaseApi.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BaseApi.Infra.Data;

public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        ApplicationDbContext context,
        IConfiguration configuration,
        ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            // Garantir que o banco de dados existe e aplicar migrations
            await _context.Database.MigrateAsync();
            _logger.LogInformation("Migrations aplicadas com sucesso");

            // Criar usuário admin se não existir
            await SeedAdminUserAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao executar seed do banco de dados");
            throw;
        }
    }

    private async Task SeedAdminUserAsync()
    {
        var adminEmail = _configuration["Admin:Email"];
        var adminName = _configuration["Admin:Name"];
        var adminPassword = _configuration["Admin:Password"];

        if (string.IsNullOrWhiteSpace(adminEmail) ||
            string.IsNullOrWhiteSpace(adminName) ||
            string.IsNullOrWhiteSpace(adminPassword))
        {
            _logger.LogWarning("Variáveis de ambiente do admin não configuradas. Pulando criação do usuário admin.");
            return;
        }

        // Verificar se já existe um usuário admin
        var adminExists = await _context.Users
            .AnyAsync(u => u.Email == adminEmail);

        if (adminExists)
        {
            _logger.LogInformation("Usuário admin já existe");
            return;
        }

        // Criar hash da senha usando BCrypt
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword);

        var adminUser = new User
        {
            Name = adminName,
            Email = adminEmail,
            PasswordHash = passwordHash,
            Role = UserRole.Admin,
            IsActive = true
        };

        _context.Users.Add(adminUser);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Usuário admin criado com sucesso: {Email}", adminEmail);
    }
}
