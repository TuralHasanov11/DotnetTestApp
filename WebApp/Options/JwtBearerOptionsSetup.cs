using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApp.Managers;

namespace WebApp.Options;

public class JwtBearerOptionsSetup : IConfigureOptions<JwtBearerOptions>
{
    private readonly JwtOptions _jwtOptions;

    public JwtBearerOptionsSetup(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }
    public void Configure(JwtBearerOptions options)
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = false, // change
            ValidateAudience = false, // change
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtOptions.ValidIssuer,
            //ValidAudience = _jwtOptions.ValidAudiences,
            //IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.Secret))
            IssuerSigningKey = new RsaSecurityKey(KeyManager.RsaKey())
        };

        //options.Configuration = new OpenIdConnectConfiguration()
        //{
        //    SigningKeys = { new RsaSecurityKey(KeyManager.RsaKey()) }
        //};
    }
}
