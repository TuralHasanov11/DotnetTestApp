using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApp.Contracts;
using WebApp.Contracts.Requests;
using WebApp.Managers;
using WebApp.Services;

namespace WebApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost(ApiRoutes.AuthenticationRoutes.Login)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authenticationService.LoginAsync(request.Email, request.Password);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost(ApiRoutes.AuthenticationRoutes.Register)]
    public async Task<IActionResult> Register([FromBody] RegistrationRequest request)
    {
        var result = await _authenticationService.RegisterAsync(request.Username, request.Email, request.Password);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet(ApiRoutes.AuthenticationRoutes.Profile)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Profile()
    {
        //var userId = User.GetId();
        var result = await _authenticationService.GetUser();

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost(ApiRoutes.AuthenticationRoutes.Refresh)]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        var result = await _authenticationService.RefreshAsync(request.AccessToken, request.RefreshToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet(ApiRoutes.AuthenticationRoutes.PublicKey)]
    public IActionResult PublicKey()
    {
        var publicKey = KeyManager.RsaPublicKey();
        var key = new RsaSecurityKey(publicKey);

        return Ok(JsonWebKeyConverter.ConvertFromRSASecurityKey(key));
    }
}
