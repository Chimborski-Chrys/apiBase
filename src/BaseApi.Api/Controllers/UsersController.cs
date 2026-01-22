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
    /// Lista todos os usuários ativos (autenticação obrigatória)
    /// Admins veem apenas usuários que criaram, exceto o admin raiz que vê todos
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        // Obter ID do usuário autenticado
        var currentUserId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "");

        // Buscar usuário atual para verificar se é raiz (sem CreatedById)
        var currentUser = await _context.Users.FindAsync(currentUserId);
        if (currentUser == null)
            return Unauthorized();

        IQueryable<User> query = _context.Users.Where(u => u.IsActive);

        // Se o usuário atual tem CreatedById (não é raiz), filtrar apenas usuários criados por ele
        if (currentUser.CreatedById.HasValue)
        {
            query = query.Where(u => u.CreatedById == currentUserId);
        }
        // Se é admin raiz (sem CreatedById), vê todos exceto ele mesmo
        else
        {
            query = query.Where(u => u.Id != currentUserId);
        }

        var users = await query.OrderBy(u => u.Name).ToListAsync();

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
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Verificar se email já existe
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == request.Email);

        if (emailExists)
        {
            return BadRequest(new { message = "Email já está cadastrado" });
        }

        // Obter ID do usuário autenticado (quem está criando)
        var currentUserId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "");

        // Criar hash da senha
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Criar novo usuário
        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = passwordHash,
            Role = request.Role,
            IsActive = true,
            CreatedById = currentUserId  // Rastrear quem criou
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Remover senha do retorno
        user.PasswordHash = "[PROTECTED]";

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    /// <summary>
    /// Atualiza um usuário (autenticação obrigatória)
    /// Admin só pode editar usuários que ele criou
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound(new { message = "Usuário não encontrado" });
        }

        // Obter ID do usuário autenticado
        var currentUserId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "");

        // Buscar usuário atual
        var currentUser = await _context.Users.FindAsync(currentUserId);
        if (currentUser == null)
            return Unauthorized();

        // Validar hierarquia: não pode editar quem o criou
        if (user.Id == currentUser.CreatedById)
        {
            return Forbid(); // 403 Forbidden
        }

        // Se não é admin raiz, só pode editar usuários que ele criou
        if (currentUser.CreatedById.HasValue && user.CreatedById != currentUserId)
        {
            return Forbid();
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
    /// Admin só pode excluir usuários que ele criou
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        // Obter ID do usuário autenticado
        var currentUserId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "");

        // Buscar usuário atual
        var currentUser = await _context.Users.FindAsync(currentUserId);
        if (currentUser == null)
            return Unauthorized();

        // Validar hierarquia: não pode excluir quem o criou
        if (user.Id == currentUser.CreatedById)
        {
            return Forbid(); // 403 Forbidden
        }

        // Se não é admin raiz, só pode excluir usuários que ele criou
        if (currentUser.CreatedById.HasValue && user.CreatedById != currentUserId)
        {
            return Forbid();
        }

        // Soft delete
        user.IsActive = false;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
