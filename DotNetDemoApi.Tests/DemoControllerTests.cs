using System;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using DotNetDemoApi.Controllers;
using DotNetDemoApi.Models;

namespace DotNetDemoApi.Tests
{
    public class DemoControllerTests
    {
        private readonly DemoController _controller;

        public DemoControllerTests()
        {
            _controller = new DemoController();
        }

        [Fact]
        public void Ping_ReturnsPong()
        {
            var result = _controller.Ping() as OkObjectResult;
            Assert.NotNull(result);
            Assert.Equal("pong", result.Value);
        }

        [Fact]
        public void Echo_ReturnsEchoedText()
        {
            var result = _controller.Echo("hello") as OkObjectResult;
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            var value = result.Value.GetType().GetProperty("echoed")?.GetValue(result.Value);
            Assert.Equal("hello", value);
        }

        [Fact]
        public void Echo_EmptyText_ReturnsEmptyPlaceholder()
        {
            var result = _controller.Echo("") as OkObjectResult;
            Assert.NotNull(result);
            var value = result.Value.GetType().GetProperty("echoed")?.GetValue(result.Value);
            Assert.Equal("(empty)", value);
        }

        [Fact]
        public void Sum_ReturnsCorrectSum()
        {
            var request = new SumRequest { A = 10, B = 20 };
            var result = _controller.Sum(request) as OkObjectResult;
            Assert.NotNull(result);
            var response = result.Value as SumResponse;
            Assert.NotNull(response);
            Assert.Equal(30, response.Result);
        }

        [Fact]
        public void Sum_NullRequest_ReturnsBadRequest()
        {
            var result = _controller.Sum(null);
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
