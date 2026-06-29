using kurswork_back.Data;
using kurswork_back.DTOs;
using kurswork_back.Models;
using kurswork_back.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MongoDB.Driver;

namespace kurswork_back.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IRegistrationRequestService _registrationRequestService;

        public AuthController(IAuthService authService, IRegistrationRequestService registrationRequestService)
        {
            _authService = authService;
            _registrationRequestService = registrationRequestService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                
                await _registrationRequestService.CreateRequestAsync(dto);
                return Ok(new { message = "Заявку надіслано. Очікуйте підтвердження менеджера." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                var result = await _authService.LoginAsync(dto);

                Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                return Ok(new
                {
                    accessToken = result.AccessToken,
                    role = result.Role
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];
                if (string.IsNullOrEmpty(refreshToken))
                    return Unauthorized(new { message = "Refresh token відсутній" });

                var result = await _authService.RefreshAsync(refreshToken);

                Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                return Ok(new { accessToken = result.AccessToken });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _authService.LogoutAsync(userId!);

            Response.Cookies.Delete("refreshToken");
            return Ok();
        }
    }
}
