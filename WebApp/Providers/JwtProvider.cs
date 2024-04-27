using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using WebApp.Authorization;
using WebApp.Contracts.Responses;
using WebApp.Data;
using WebApp.Domain;
using WebApp.Managers;
using WebApp.Options;

namespace WebApp.Providers;

public sealed class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _jwtOptions;
    private readonly ApplicationDbContext _dbContext;
    private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly IPermissionService _permissionService;

    public JwtProvider(
        IOptions<JwtOptions> jwtOptions,
        ApplicationDbContext dbContext,
        TokenValidationParameters tokenValidationParameters,
        IPermissionService permissionService)
    {
        _jwtOptions = jwtOptions.Value;
        _dbContext = dbContext;
        _tokenValidationParameters = tokenValidationParameters;
        _permissionService = permissionService;
    }

    public async Task<TokenResponse> CreateToken(ApplicationUser user, IEnumerable<Claim> claims)
    {
        claims = claims.Concat(await GenerateJwtClaims(user));

        var handler = new JwtSecurityTokenHandler();

        var token = handler.CreateToken(new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddMinutes(Convert.ToDouble(_jwtOptions.AccessTokenLifeTime)),
            SigningCredentials = new SigningCredentials(new RsaSecurityKey(KeyManager.RsaKey()), SecurityAlgorithms.RsaSha256),
            //SigningCredentials = new SigningCredentials(
            //    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.Secret)),
            //    SecurityAlgorithms.HmacSha256Signature),
        });

        var refreshToken = GenerateRefreshToken(user, token);

        await _dbContext.RefreshTokens.AddAsync(refreshToken);
        await _dbContext.SaveChangesAsync();

        return new TokenResponse(handler.WriteToken(token), refreshToken.Token);
    }

    private async Task<IEnumerable<Claim>> GenerateJwtClaims(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Name, user.UserName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Email),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new("id", user.Id.ToString())
        };

        HashSet<string> permissions = await _permissionService.GetPermissionsAsync(user.Id);

        foreach (var permission in permissions)
        {
            claims.Add(new(ApplicationClaims.Permissions, permission));
        }

        return claims;
    }

    public ClaimsPrincipal? GetPrincipalFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.RsaSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    private RefreshToken GenerateRefreshToken(ApplicationUser user, SecurityToken token)
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();

        rng.GetBytes(randomNumber);

        return new RefreshToken
        {
            CreationDate = DateTime.UtcNow,
            UserId = user.Id,
            JwtId = token.Id,
            ExpiryDate = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenLifeTime),
            Token = Convert.ToBase64String(randomNumber)
        };
    }
}