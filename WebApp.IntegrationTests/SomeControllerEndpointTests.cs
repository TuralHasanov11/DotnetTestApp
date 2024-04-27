using FluentAssertions;
using System.Security.Claims;

namespace WebApp.IntegrationTests;

[Collection("AppTestCollection")]
public class SomeControllerEndpointTests : IAsyncLifetime
{
    private readonly AppInstance _instance;
    private readonly Func<Task> _resetDatabase;

    public SomeControllerEndpointTests(AppInstance instance)
    {
        _instance = instance;
        _resetDatabase = _instance.ResetDatabaseAsync;
    }

    //[Theory]
    //[InlineData("/api/Some/public")]
    //public async Task Get_EndpointsReturnSuccess(string url)
    //{
    //    var client = _instance.HttpClient;

    //    var response = await client.GetAsync(url);

    //    response.EnsureSuccessStatusCode();
    //    var content = await response.Content.ReadAsStringAsync();
    //    await Verify(content);
    //}

    //[Fact]
    //public async Task Get_SecureEndpointReturnSuccess()
    //{
    //    var client = _instance.HttpClient;

    //    var response = await client.GetAsync("/api/Some/secure");

    //    response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    //}

    //[Fact]
    //public async Task Get_PrivilegedEndpointReturnSuccess()
    //{
    //    var client = _instance
    //        .AuthenticatedInstance(new Claim("Role", "Customer"))
    //        .CreateClient(new()
    //        {
    //            AllowAutoRedirect = false
    //        });

    //    var response = await client.GetAsync("/api/Some/privileged");

    //    response.EnsureSuccessStatusCode();
    //    var content = await response.Content.ReadAsStringAsync();
    //    await Verify(new { content, response });
    //}

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();

    //[Fact]
    //public async Task Get_AdministratorEndpointReturnSuccess()
    //{
    //    var client = _instance
    //        .AuthenticatedInstance(new Claim("Role", "Administrator"))
    //        .CreateClient(new()
    //        {
    //            AllowAutoRedirect = false
    //        });

    //    var response = await client.GetAsync("/api/Some/administrator");

    //    response.EnsureSuccessStatusCode();
    //    var content = await response.Content.ReadAsStringAsync();
    //    await Verify(content);
    //}
}