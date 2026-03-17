using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using DotNetDemoApi;

namespace DotNetDemoApi.Tests
{
    /// <summary>
    /// Security-focused tests: response headers, input validation, and safe handling of malicious-looking input.
    /// </summary>
    public class SecurityTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public SecurityTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task SecurityHeaders_PresentOnAllResponses()
        {
            var response = await _client.GetAsync("/api/demo/ping");

            Assert.True(response.Headers.TryGetValues("X-Content-Type-Options", out var nosniff));
            Assert.Contains("nosniff", nosniff);

            Assert.True(response.Headers.TryGetValues("X-Frame-Options", out var frameOpts));
            Assert.Contains("DENY", frameOpts);

            Assert.True(response.Headers.TryGetValues("Referrer-Policy", out var referrer));
            Assert.Contains("strict-origin-when-cross-origin", referrer);

            Assert.True(response.Headers.TryGetValues("X-XSS-Protection", out var xss));
            Assert.Contains("1; mode=block", xss);
        }

        [Fact]
        public async Task CreateProduct_NameTooLong_ReturnsBadRequest()
        {
            var longName = new string('x', 201);
            var body = JsonSerializer.Serialize(new { name = longName, price = 10 });
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/products", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateProduct_NegativePrice_ReturnsBadRequest()
        {
            var body = JsonSerializer.Serialize(new { name = "Valid", price = -1 });
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/products", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateProduct_XssPayloadInName_AcceptedAsData_NoCrash()
        {
            var body = JsonSerializer.Serialize(new { name = "<script>alert(1)</script>", price = 0 });
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/products", content);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var json = await response.Content.ReadAsStringAsync();
            Assert.Contains("script", json);
        }

        [Fact]
        public async Task Sum_InvalidJson_ReturnsBadRequest()
        {
            var content = new StringContent("not valid json", Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/demo/sum", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Sum_MissingBody_ReturnsBadRequest()
        {
            var content = new StringContent("", Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/demo/sum", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Echo_ReflectedContent_ReturnedAsJsonEscaped()
        {
            var payload = "'; DROP TABLE products;--";
            var response = await _client.GetAsync("/api/demo/echo?text=" + System.Uri.EscapeDataString(payload));

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            Assert.Contains("echoed", json);
            Assert.Contains("DROP", json);
        }
    }
}
