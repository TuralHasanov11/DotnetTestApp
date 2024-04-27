using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using Respawn;
using System.Data.Common;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Testcontainers.PostgreSql;
using WebApp.Data;

namespace WebApp.IntegrationTests;

public class AppInstance : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .Build();

    public HttpClient HttpClient { get; private set; } = default!;
    private DbConnection _dbConnection = default!;
    private Respawner _respawner = default!;

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

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        _dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());
        HttpClient = CreateClient(new()
        {
            AllowAutoRedirect = false
        });
        await InitializeRespawner();
    }

    private async Task InitializeRespawner()
    {
        await _dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["application"]
        });
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<ApplicationDbContext>));

            services.Remove(dbContextDescriptor);

            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbConnection));

            services.Remove(dbConnectionDescriptor);

            services.AddSingleton<DbConnection>(container =>
            {
                var connection = new NpgsqlConnection(_dbContainer.GetConnectionString());
                connection.Open();

                return connection;
            });

            services.AddDbContext<ApplicationDbContext>((container, options) =>
            {
                var connection = container.GetRequiredService<DbConnection>();
                options.UseNpgsql(connection);
            });
        });

        builder.UseEnvironment("Development");
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
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