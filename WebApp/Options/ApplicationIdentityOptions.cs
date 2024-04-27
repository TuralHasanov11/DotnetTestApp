using System.ComponentModel.DataAnnotations;

namespace WebApp.Options;

public sealed class ApplicationIdentityOptions
{
    [Required]
    [Range(1, 255)]
    public int StoresMaxLengthForKeys { get; set; }

    [Required]
    [Range(6, 50)]
    public int PasswordRequiredLength { get; set; }

    [Required]
    [Range(1, 20)]
    public int PasswordRequiredUniqueChars { get; set; }

    [Required]
    [Range(1, 1000)]
    public int LockoutDefaultLockoutTimeSpan { get; set; }

    [Required]
    [Range(3, 20)]
    public int LockoutMaxFailedAccessAttempts { get; set; }

    [Required]
    public string UserAllowedUserNameCharacters { get; set; }

}