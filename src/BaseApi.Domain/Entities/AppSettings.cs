namespace BaseApi.Domain.Entities;

/// <summary>
/// Configurações globais da aplicação para White-Label
/// Apenas 1 registro deve existir na tabela
/// </summary>
public class AppSettings : BaseEntity
{
    /// <summary>
    /// Nome da marca/empresa
    /// </summary>
    public string BrandName { get; set; } = "Base API";

    /// <summary>
    /// URL ou Base64 da logo
    /// </summary>
    public string? LogoUrl { get; set; }

    /// <summary>
    /// Cor primária em formato HEX (#RRGGBB)
    /// </summary>
    public string PrimaryColor { get; set; } = "#3B82F6"; // Azul

    /// <summary>
    /// Cor secundária em formato HEX (#RRGGBB)
    /// </summary>
    public string SecondaryColor { get; set; } = "#8B5CF6"; // Roxo

    /// <summary>
    /// Cor de destaque/accent em formato HEX (#RRGGBB)
    /// </summary>
    public string AccentColor { get; set; } = "#22C55E"; // Verde
}
