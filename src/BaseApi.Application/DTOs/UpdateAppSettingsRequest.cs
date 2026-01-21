using System.ComponentModel.DataAnnotations;

namespace BaseApi.Application.DTOs;

public class UpdateAppSettingsRequest
{
    [Required(ErrorMessage = "Nome da marca é obrigatório")]
    [StringLength(100, ErrorMessage = "Nome da marca deve ter no máximo 100 caracteres")]
    public string BrandName { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "URL da logo deve ter no máximo 500 caracteres")]
    public string? LogoUrl { get; set; }

    [Required(ErrorMessage = "Cor primária é obrigatória")]
    [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Cor primária deve estar no formato #RRGGBB")]
    public string PrimaryColor { get; set; } = string.Empty;

    [Required(ErrorMessage = "Cor secundária é obrigatória")]
    [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Cor secundária deve estar no formato #RRGGBB")]
    public string SecondaryColor { get; set; } = string.Empty;

    [Required(ErrorMessage = "Cor de destaque é obrigatória")]
    [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Cor de destaque deve estar no formato #RRGGBB")]
    public string AccentColor { get; set; } = string.Empty;
}
