using System.ComponentModel.DataAnnotations;

namespace WebApp.Options;

public sealed class JwtOptions
{
    [Required]
    public string Secret { get; set; }

    [Required, Url]
    public string ValidAudiences { get; set; }

    [Required]
    public string ValidIssuer { get; set; }

    [Required]
    [Range(10, 500)]
    public int AccessTokenLifeTime { get; set; }

    [Required]
    [Range(1, 500)]
    public int RefreshTokenLifeTime { get; set; }
}