namespace WebApp.Contracts.Requests;

public sealed record RegistrationRequest(string Username, string Email, string Password);