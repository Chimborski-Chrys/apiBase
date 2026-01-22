using BaseApi.Application.DTOs;
using BaseApi.Application.Interfaces;
using BaseApi.Infra.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppSettingsController : ControllerBase
{
    private readonly IAppSettingsService _settingsService;
    private readonly ApplicationDbContext _context;

    public AppSettingsController(IAppSettingsService settingsService, ApplicationDbContext context)
    {
        _settingsService = settingsService;
        _context = context;
    }

    /// <summary>
    /// Obtém as configurações da aplicação (público - para white-label)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<AppSettingsResponse>> GetSettings()
    {
        var settings = await _settingsService.GetSettingsAsync();

        if (settings == null)
            return NotFound(new { message = "Configurações não encontradas" });

        return Ok(settings);
    }

    /// <summary>
    /// Atualiza as configurações da aplicação (apenas admin raiz)
    /// Somente o primeiro admin (sem CreatedById) pode alterar as configurações do sistema
    /// </summary>
    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AppSettingsResponse>> UpdateSettings([FromBody] UpdateAppSettingsRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Obter ID do usuário autenticado
        var currentUserId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "");

        // Buscar usuário atual para verificar se é admin raiz
        var currentUser = await _context.Users.FindAsync(currentUserId);
        if (currentUser == null)
            return Unauthorized();

        // Apenas admin raiz (sem CreatedById) pode alterar configurações do sistema
        if (currentUser.CreatedById.HasValue)
        {
            return Forbid(); // 403 Forbidden
        }

        try
        {
            var updated = await _settingsService.UpdateSettingsAsync(request);
            return Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
