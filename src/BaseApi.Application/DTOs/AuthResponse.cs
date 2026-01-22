using BaseApi.Domain.Enums;

namespace BaseApi.Application.DTOs;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime ExpiresAt { get; set; }
    public Guid? CreatedById { get; set; }  // null = admin raiz
}
