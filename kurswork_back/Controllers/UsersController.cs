using kurswork_back.DTOs;
using kurswork_back.Models;
using kurswork_back.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace kurswork_back.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _userService.GetAllAsync();

                var result = users.Select(u => new UserDto
                {
                    Id = u.Id!,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);

                if (user == null)
                    return NotFound();

                var result = new UserDto
                {
                    Id = user.Id!,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto user)
        {           
            try
            {
                await _userService.CreateAsync(user);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (currentUserId == id)
                    return BadRequest(new { message = "Не можна видалити свій власний акаунт" });
    
                await _userService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id,[FromBody] CreateUserDto user)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (currentUserId == id)
                {
                    var currentUser = await _userService.GetByIdAsync(id);
                    if (currentUser != null && currentUser.Role != user.Role)
                        return BadRequest(new { message = "Не можна змінювати власну роль" });
                }

                var updated = await _userService.UpdateAsync(id, user);

                if (!updated)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(string id, [FromBody] UpdateUserDto dto)
        {

            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (currentUserId == id && !string.IsNullOrWhiteSpace(dto.Role))
                {
                    var currentUser = await _userService.GetByIdAsync(id);
                    if (currentUser != null && currentUser.Role != dto.Role)
                        return BadRequest(new { message = "Не можна змінювати власну роль" });
                }

                var updated = await _userService.PatchAsync(id, dto);

                if (!updated)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}