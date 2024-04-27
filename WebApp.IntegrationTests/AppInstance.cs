using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace WebApp.IntegrationTests;

public class AppInstance : WebApplicationFactory<Program>
{
    public WebApplicationFactory<Program> AuthenticatedInstance(params Claim[] claimSeed)
    {
        return WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IAuthenticationSchemeProvider, MockSchemeProvider>();
                services.AddSingleton<MockClaimSeed>(_ => new(claimSeed));
            });
        });
    }

    public class MockSchemeProvider : AuthenticationSchemeProvider
    {
        public MockSchemeProvider(IOptions<AuthenticationOptions> options) : base(options)
        {
        }

        protected MockSchemeProvider(IOptions<AuthenticationOptions> options, IDictionary<string, AuthenticationScheme> schemes) : base(options, schemes)
        {
        }

        public override Task<AuthenticationScheme> GetDefaultAuthenticateSchemeAsync()
        {
            AuthenticationScheme mockScheme = new(
                IdentityConstants.ApplicationScheme,
                IdentityConstants.ApplicationScheme,
                typeof(MockAuthenticationHandler));
            return Task.FromResult(mockScheme);
        }

        public override Task<AuthenticationScheme> GetSchemeAsync(string name)
        {
            AuthenticationScheme mockScheme = new(
                IdentityConstants.ApplicationScheme,
                IdentityConstants.ApplicationScheme,
                typeof(MockAuthenticationHandler));
            return Task.FromResult(mockScheme);
        }
    }

    public class MockAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly MockClaimSeed _claimSeed;

        public MockAuthenticationHandler(
            MockClaimSeed claimSeed,
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
            _claimSeed = claimSeed;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = _claimSeed.Seed();
            var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, IdentityConstants.ApplicationScheme);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    public class MockClaimSeed
    {
        private readonly IEnumerable<Claim> _seed;

        public MockClaimSeed(IEnumerable<Claim> seed)
        {
            _seed = seed;
        }

        public IEnumerable<Claim> Seed() => _seed;
    }
}