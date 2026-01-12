using System.ComponentModel.DataAnnotations;
using BaseApi.Domain.Enums;

namespace BaseApi.Application.DTOs;

public class UpdateUserRequest
{
    [MinLength(3, ErrorMessage = "Nome deve ter no mínimo 3 caracteres")]
    [MaxLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
    public string? Name { get; set; }

    [EmailAddress(ErrorMessage = "Email inválido")]
    [MaxLength(200, ErrorMessage = "Email deve ter no máximo 200 caracteres")]
    public string? Email { get; set; }

    public string? PasswordHash { get; set; }

    public UserRole? Role { get; set; }

    public bool? IsActive { get; set; }
}
