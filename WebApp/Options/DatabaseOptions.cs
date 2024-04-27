using System.ComponentModel.DataAnnotations;

namespace WebApp.Options;

public sealed class DatabaseOptions
{

    [Required]
    [Range(1, 20)]
    public int MaxRetryCount { get; set; }

    [Required]
    [Range(1, 60)]
    public int CommandTimeout { get; set; }

    [Required]
    public bool EnableDetailedErrors { get; set; }
}