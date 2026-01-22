namespace BaseApi.Application.DTOs;

public class AppSettingsResponse
{
    public Guid Id { get; set; }
    public string BrandName { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string PrimaryColor { get; set; } = string.Empty;
    public string SecondaryColor { get; set; } = string.Empty;
    public string AccentColor { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}
