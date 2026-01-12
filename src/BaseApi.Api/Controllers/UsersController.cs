using BaseApi.Application.DTOs;
using BaseApi.Domain.Entities;
using BaseApi.Infra.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BaseApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lista todos os usuários ativos (público)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var users = await _context.Users
            .Where(u => u.IsActive)
            .OrderBy(u => u.Name)
            .ToListAsync();

        return Ok(users);
    }

    /// <summary>
    /// Lista todos os usuários inativos (autenticação obrigatória)
    /// Requer token JWT - Clique em "Authorize" no topo do Swagger
    /// </summary>
    [HttpGet("inactive")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<User>>> GetInactiveUsers()
    {
        var users = await _context.Users
            .Where(u => !u.IsActive)
            .OrderBy(u => u.Name)
            .ToListAsync();

        return Ok(users);
    }

    /// <summary>
    /// Busca um usuário por ID (público)
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<User>> GetUser(Guid id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null || !user.IsActive)
        {
            return NotFound();
        }

        return Ok(user);
    }

    /// <summary>
    /// Cria um novo usuário (autenticação obrigatória)
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<User>> CreateUser(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    /// <summary>
    /// Atualiza um usuário (autenticação obrigatória)
    /// Requer token JWT - Clique em "Authorize" no topo do Swagger
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound(new { message = "Usuário não encontrado" });
        }

        // Atualizar apenas os campos fornecidos (não nulos)
        if (request.Name != null)
            user.Name = request.Name;

        if (request.Email != null)
            user.Email = request.Email;

        if (request.Role.HasValue)
            user.Role = request.Role.Value;

        if (request.IsActive.HasValue)
            user.IsActive = request.IsActive.Value;

        if (!string.IsNullOrWhiteSpace(request.PasswordHash))
            user.PasswordHash = request.PasswordHash;

        user.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Users.AnyAsync(e => e.Id == id))
            {
                return NotFound();
            }
            throw;
        }

        return Ok(user);
    }

    /// <summary>
    /// Desativa um usuário (autenticação obrigatória)
    /// Requer token JWT - Clique em "Authorize" no topo do Swagger
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        // Soft delete
        user.IsActive = false;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
