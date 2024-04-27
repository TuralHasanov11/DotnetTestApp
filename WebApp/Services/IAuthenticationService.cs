using WebApp.Contracts.Requests;
using WebApp.Contracts.Responses;
using WebApp.Domain.Shared;

namespace WebApp.Services;

public interface IAuthenticationService
{
    Task<Result<RegistrationResponse>> RegisterAsync(string username, string email, string password);
    Task<Result<TokenResponse>> LoginAsync(string email, string password);
    Task<Result<UserResponse>> GetUser();

    Task<Result<TokenResponse>> RefreshAsync(string accessToken, string refreshToken);
}
