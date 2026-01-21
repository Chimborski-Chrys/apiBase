using BaseApi.Application.DTOs;
using BaseApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppSettingsController : ControllerBase
{
    private readonly IAppSettingsService _settingsService;

    public AppSettingsController(IAppSettingsService settingsService)
    {
        _settingsService = settingsService;
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
    /// Atualiza as configurações da aplicação (apenas admin)
    /// </summary>
    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AppSettingsResponse>> UpdateSettings([FromBody] UpdateAppSettingsRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

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
