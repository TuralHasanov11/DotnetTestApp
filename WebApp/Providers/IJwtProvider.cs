using System.Security.Claims;
using WebApp.Contracts.Responses;
using WebApp.Domain;

namespace WebApp.Providers;

public interface IJwtProvider
{
    Task<TokenResponse> CreateToken(ApplicationUser user, IEnumerable<Claim> claims);
    ClaimsPrincipal? GetPrincipalFromToken(string token);
}