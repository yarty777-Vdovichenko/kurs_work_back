using kurswork_back.DTOs;
using kurswork_back.Models;
using kurswork_back.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize(Roles = $"{Roles.Manager},{Roles.Admin}")]
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
        catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
    }

    [Authorize(Roles = $"{Roles.Manager},{Roles.Admin}")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        try
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(new UserDto { Id = user.Id!, Name = user.Name, Email = user.Email, Role = user.Role });
        }
        catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
    }

    [Authorize(Roles = $"{Roles.Manager},{Roles.Admin}")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            var currentRole = User.FindFirstValue(ClaimTypes.Role)!;
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            if (currentUserId == id)
                return BadRequest(new { message = "Не можна видалити свій власний акаунт" });

            var targetUser = await _userService.GetByIdAsync(id);
            if (targetUser == null) return NotFound();

            if (currentRole == Roles.Admin && targetUser.Role != Roles.User)
                return Forbid();

            if (currentRole == Roles.Manager && targetUser.Role == Roles.Manager)
                return Forbid();

            await _userService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
    }

    [Authorize(Roles = Roles.Manager)]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CreateUserDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            if (currentUserId == id)
                return BadRequest(new { message = "Не можна редагувати свій власний акаунт" });

            var targetUser = await _userService.GetByIdAsync(id);
            if (targetUser == null) return NotFound();

            if (dto.Role == Roles.Manager)
                return Forbid();

            var updated = await _userService.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }
        catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
    }
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _userService.CreateAsync(dto);
            return Ok(new { message = "Користувача створено" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    [Authorize(Roles = Roles.Manager)]
    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(string id, [FromBody] UpdateUserDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            if (currentUserId == id)
                return BadRequest(new { message = "Не можна редагувати свій власний акаунт" });

            var targetUser = await _userService.GetByIdAsync(id);
            if (targetUser == null) return NotFound();

            if (dto.Role == Roles.Manager)
                return Forbid();

            var updated = await _userService.PatchAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }
        catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
    }
}