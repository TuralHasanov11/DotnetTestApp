using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace WebApp.Options
{
    public class ApplicationIdentityOptionsSetup : IConfigureOptions<IdentityOptions>
    {
        private readonly ApplicationIdentityOptions _applicationIdentityOptions;
        public ApplicationIdentityOptionsSetup(IOptions<ApplicationIdentityOptions> applicationIdentityOptions)
        {
            _applicationIdentityOptions = applicationIdentityOptions.Value;
        }
        public void Configure(IdentityOptions options)
        {
            options.Stores.MaxLengthForKeys = _applicationIdentityOptions.StoresMaxLengthForKeys;

            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;

            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = _applicationIdentityOptions.PasswordRequiredLength;
            options.Password.RequiredUniqueChars = _applicationIdentityOptions.PasswordRequiredUniqueChars;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(_applicationIdentityOptions.LockoutDefaultLockoutTimeSpan);
            options.Lockout.MaxFailedAccessAttempts = _applicationIdentityOptions.LockoutMaxFailedAccessAttempts;
            options.Lockout.AllowedForNewUsers = true;

            options.User.AllowedUserNameCharacters = _applicationIdentityOptions.UserAllowedUserNameCharacters;
            options.User.RequireUniqueEmail = true;
        }
    }
}
