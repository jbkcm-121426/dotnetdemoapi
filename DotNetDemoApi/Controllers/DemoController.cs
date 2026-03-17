using System;
using Microsoft.AspNetCore.Mvc;
using DotNetDemoApi.Models;

namespace DotNetDemoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DemoController : ControllerBase
    {
        /// <summary>
        /// Health check - returns pong.
        /// </summary>
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("pong");
        }

        /// <summary>
        /// Echoes back the provided text.
        /// </summary>
        [HttpGet("echo")]
        public IActionResult Echo([FromQuery] string text = "")
        {
            return Ok(new { echoed = string.IsNullOrEmpty(text) ? "(empty)" : text });
        }

        /// <summary>
        /// Returns the sum of two numbers.
        /// </summary>
        [HttpPost("sum")]
        public IActionResult Sum([FromBody] SumRequest request)
        {
            if (request == null)
                return BadRequest("Request body is required.");
            return Ok(new SumResponse { Result = request.A + request.B });
        }
    }
}
