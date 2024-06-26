using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebApp.Contracts.Responses;
using WebApp.Data;
using WebApp.Domain;
using WebApp.Domain.Shared;
using WebApp.Extensions;
using WebApp.Providers;

namespace WebApp.Services;

public sealed class AuthenticationService : IAuthenticationService
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtProvider _jwtProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _dbContext;

    public AuthenticationService(
        UserManager<ApplicationUser> userManager,
        IJwtProvider jwtProvider,
        SignInManager<ApplicationUser> signInManager,
        IHttpContextAccessor httpContextAccessor,
        ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _jwtProvider = jwtProvider;
        _signInManager = signInManager;
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContext;
    }

    public async Task<Result<TokenResponse>> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return Result.Failure<TokenResponse>(new Error("User.InvalidCredentials", "Email or Password is incorrect", ErrorType.Conflict));
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, true);

        if (!result.Succeeded)
        {
            return Result.Failure<TokenResponse>(new Error("User.InvalidCredentials", "Email or Password is incorrect", ErrorType.Conflict));
        }

        var token = await _jwtProvider.CreateToken(user, await GetClaimsFomUser(user));

        if (token is null)
        {
            return Result.Failure<TokenResponse>(new Error("Token.CannotBeCreated", "Token cannot be created", ErrorType.Failure));
        }

        return Result.Success(token);
    }

    private async Task<IList<Claim>> GetClaimsFomUser(ApplicationUser? user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var claims = await _userManager.GetClaimsAsync(user);

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }

    public async Task<Result<RegistrationResponse>> RegisterAsync(string username, string email, string password)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser is not null)
        {
            return Result.Failure<RegistrationResponse>(new Error("User.AlreadyExists", "User already exists", ErrorType.Conflict));
        }

        var newUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = username,
        };

        var result = await _userManager.CreateAsync(newUser, password);

        if (!result.Succeeded)
        {
            return Result.Failure<RegistrationResponse>(new Error("User.CannotBeCreated", "User cannot be created", ErrorType.Conflict));
        }

        await _userManager.AddClaimAsync(newUser, new Claim("Course.View", "True"));

        return Result.Success(new RegistrationResponse());
    }

    public async Task<Result<UserResponse>> GetUser()
    {
        var user = await _userManager.FindByIdAsync(_httpContextAccessor.HttpContext.User.GetId());

        if (user is null)
        {
            return Result.Failure<UserResponse>(new Error("User.NotFound", "User not found", ErrorType.NotFound));
        }

        return Result.Success(new UserResponse(user.Id, user.UserName, user.Email));
    }

    public async Task<Result<TokenResponse>> RefreshAsync(string token, string refreshToken)
    {
        var validatedToken = _jwtProvider.GetPrincipalFromToken(token);

        if (validatedToken is null)
        {
            return Result.Failure<TokenResponse>(new Error("User.InvalidToken", "Invalid Token", ErrorType.Validation));
        }

        var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

        var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            .AddSeconds(expiryDateUnix);

        if (expiryDateTimeUtc > DateTime.UtcNow)
        {
            return Result.Failure<TokenResponse>(new Error("User.TokenNotExpired", "Token has not expired", ErrorType.Validation));
        }

        var storedRefreshToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken);

        if (storedRefreshToken is null)
        {
            return Result.Failure<TokenResponse>(new Error("User.TokenNotFound", "Token has not been found", ErrorType.NotFound));
        }

        if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
        {
            return Result.Failure<TokenResponse>(new Error("User.TokenExpired", "Token has expired", ErrorType.Validation));
        }

        if (storedRefreshToken.Invalidated)
        {
            return Result.Failure<TokenResponse>(new Error("User.TokenInvalidated", "Token has been invalidated", ErrorType.Validation));
        }

        if (storedRefreshToken.Used)
        {
            return Result.Failure<TokenResponse>(new Error("User.TokenUsed", "Token was used", ErrorType.Validation));
        }

        var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

        if (storedRefreshToken.JwtId != jti)
        {
            return Result.Failure<TokenResponse>(new Error("User.TokenDoesNotMatch", "Token does not match", ErrorType.Validation));
        }

        storedRefreshToken.Used = true;
        _dbContext.RefreshTokens.Update(storedRefreshToken);
        await _dbContext.SaveChangesAsync();

        var user = await _userManager.FindByIdAsync(_httpContextAccessor.HttpContext.User.GetId());

        var tokenResult = await _jwtProvider.CreateToken(user, await GetClaimsFomUser(user));

        if (tokenResult is null)
        {
            return Result.Failure<TokenResponse>(new Error("Token.CannotBeCreated", "Token cannot be created", ErrorType.Failure));
        }

        return Result.Success(tokenResult);
    }
}