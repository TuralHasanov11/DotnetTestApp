using System.Net;
using WebApp.Controllers;

namespace WebApp.IntegrationTests
{
    public class SomeControllerEndpointTests : IClassFixture<AppInstance>
    {
        [Fact]
        public async void PublicTest()
        {
            var controller = new SomeController();
            var client = new HttpClient()
            {
                BaseAddress = new Uri("https://localhost:7109")
            };

            var result = await client.GetAsync("/public");

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            var content = await result.Content.ReadAsStringAsync();
            Assert.Equal("Public", content);
        }

        [Fact]
        public async void SecureTest()
        {
            var controller = new SomeController();
            var client = new HttpClient()
            {
                BaseAddress = new Uri("https://localhost:7109")
            };

            var result = await client.GetAsync("/secure");

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            var content = await result.Content.ReadAsStringAsync();
            Assert.Equal("Secure", content);
        }
    }
}