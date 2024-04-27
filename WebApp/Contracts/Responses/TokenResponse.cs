namespace WebApp.Contracts.Responses;

public sealed record TokenResponse(string AccessToken, string RefreshToken);