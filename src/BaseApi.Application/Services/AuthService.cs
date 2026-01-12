using BaseApi.Application.DTOs;
using BaseApi.Application.Interfaces;
using BaseApi.Domain.Entities;
using BCrypt.Net;

namespace BaseApi.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null || !user.IsActive)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        var token = _tokenService.GenerateToken(user);
        var expireMinutes = 60; // Default

        return new AuthResponse
        {
            Token = token,
            Email = user.Email,
            Name = user.Name,
            Role = user.Role,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expireMinutes)
        };
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
            return null;

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            IsActive = true
        };

        await _userRepository.AddAsync(user);

        var token = _tokenService.GenerateToken(user);
        var expireMinutes = 60; // Default

        return new AuthResponse
        {
            Token = token,
            Email = user.Email,
            Name = user.Name,
            Role = user.Role,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expireMinutes)
        };
    }
}
