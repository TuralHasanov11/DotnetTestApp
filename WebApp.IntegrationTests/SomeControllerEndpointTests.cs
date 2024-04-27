using FluentAssertions;
using System.Security.Claims;

namespace WebApp.IntegrationTests;

public class SomeControllerEndpointTests : IClassFixture<AppInstance>
{
    private readonly AppInstance _instance;

    public SomeControllerEndpointTests(AppInstance instance)
    {
        _instance = instance;
    }

    [Theory]
    [InlineData("/public")]
    public async Task Get_EndpointsReturnSuccess(string url)
    {
        var client = _instance.CreateClient(new()
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        await Verify(content);
    }

    [Fact]
    public async Task Get_SecureEndpointReturnSuccess()
    {
        var client = _instance
            .AuthenticatedInstance()
            .CreateClient(new()
            {
                AllowAutoRedirect = false
            });

        var response = await client.GetAsync("/secure");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        await Verify(content);
    }

    [Fact]
    public async Task Get_PrivilegedEndpointReturnSuccess()
    {
        var client = _instance
            .AuthenticatedInstance(new Claim("Role", "Customer"))
            .CreateClient(new()
            {
                AllowAutoRedirect = false
            });

        var response = await client.GetAsync("/privileged");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        await Verify(new { content, response });
    }

    [Fact]
    public async Task Get_AdministratorEndpointReturnSuccess()
    {
        var client = _instance
            .AuthenticatedInstance(new Claim("Role", "Administrator"))
            .CreateClient(new()
            {
                AllowAutoRedirect = false
            });

        var response = await client.GetAsync("/administrator");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        await Verify(content);
    }
}