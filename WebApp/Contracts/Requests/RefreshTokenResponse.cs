namespace WebApp.Contracts.Requests;

public sealed record  RefreshTokenResponse(string AccessToken, string RefreshToken);
