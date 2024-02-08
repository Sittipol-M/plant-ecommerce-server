using Microsoft.AspNetCore.Mvc;
using plant_ecommerce_server.Requests;
using plant_ecommerce_server.Responses;
using plant_ecommerce_server.Services;

namespace plant_ecommerce_server.Controllers
{
    [ApiController]
    [Route("authentication")]
    public class AuthenController : ControllerBase
    {
        private readonly IAuthenService _authenService;
        public AuthenController(IAuthenService authenService)
        {
            _authenService = authenService;
        }
        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromBody] LoginBodyRequest bodyRequest)
        {
            LoginResponse response = await _authenService.Login(bodyRequest);
            Response.Cookies.Append("refresh-token", response.RefreshToken, new()
            {
                HttpOnly = true
            });
            return Ok(response);
        }
        [HttpPost("/register")]
        public async Task<IActionResult> Register([FromBody] RegisterBodyRequest bodyRequest)
        {
            await _authenService.Register(bodyRequest);
            return Ok();
        }
        [HttpPost("/refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenBodyRequest bodyRequest)
        {
            var refreshToken = Request.Cookies["refresh-token"];
            var response = await _authenService.RefreshToken(bodyRequest, refreshToken);
            Response.Cookies.Delete("refresh-token");
            Response.Cookies.Append("refresh-token", response.RefreshToken, new()
            {
                HttpOnly = true
            });
            return Ok(response);
        }
        [HttpPost("/logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutBodyRequest bodyRequest)
        {
            await _authenService.Logout(bodyRequest);
            Response.Cookies.Delete("refresh-token");
            return Ok();
        }
    }
}