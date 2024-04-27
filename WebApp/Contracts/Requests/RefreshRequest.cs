namespace WebApp.Contracts.Requests;

public sealed record RefreshRequest(string AccessToken, string RefreshToken);