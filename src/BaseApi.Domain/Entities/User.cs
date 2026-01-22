using BaseApi.Domain.Enums;

namespace BaseApi.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
    public bool IsActive { get; set; } = true;

    // Hierarquia de administradores - rastreia quem criou este usuário
    public Guid? CreatedById { get; set; }
    public User? CreatedBy { get; set; }
}
