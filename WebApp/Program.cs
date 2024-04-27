using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApp.Authorization;
using WebApp.Data;
using WebApp.Domain;
using WebApp.Managers;
using WebApp.Options;
using WebApp.Providers;
using WebApp.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key access to API",
        Name = "x-api-key",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
            },
            new List<string>()
        }
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                },
            },
            new List<string>()
        }
    });

    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Web", Version = "v1" });
});



builder.Services.AddHttpContextAccessor();


builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddRoles<ApplicationRole>()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.ConfigureOptions<JwtOptionsSetup>();
builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();


var tokenValidationParameters = new TokenValidationParameters()
{
    ValidateIssuer = false, // change
    ValidateAudience = false, // change
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    //ValidIssuer = _jwtOptions.ValidIssuer,
    //ValidAudience = _jwtOptions.ValidAudiences,
    IssuerSigningKey = new RsaSecurityKey(KeyManager.RsaKey())
};

builder.Services.AddSingleton(tokenValidationParameters);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = tokenValidationParameters;
});
//.AddBearerToken(IdentityConstants.BearerScheme);

builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrator", policy =>
        policy.RequireClaim("Role", "Administrator"));

    options.AddPolicy("Customer", policy =>
        policy.RequireClaim("Role", "Customer"));

    options.AddPolicy("CourseView", policy =>
        policy.RequireClaim("Course.View", "True"));
});

builder.Services.AddAuthorizationBuilder();

builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();


builder.Services.AddOptions<DatabaseOptions>()
    .BindConfiguration("DatabaseOptions")
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddDbContext<ApplicationDbContext>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
//app.MapIdentityApi<ApplicationUser>();

app.Run();


var runTimeContextData1 = AppContext.GetData("abc");
Console.WriteLine(runTimeContextData1);

public partial class Program
{ }