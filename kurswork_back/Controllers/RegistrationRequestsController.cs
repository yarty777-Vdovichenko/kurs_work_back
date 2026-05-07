using kurswork_back.Models;
using kurswork_back.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace kurswork_back.Controllers
{
    [ApiController]
    [Route("api/registration-requests")]
    public class RegistrationRequestsController : ControllerBase
    {
        private readonly IRegistrationRequestService _service;

        public RegistrationRequestsController(IRegistrationRequestService service)
        {
            _service = service;
        }

        [Authorize(Roles = $"{Roles.Manager}")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? status = null)
        {
            try
            {
                var result = await _service.GetAllAsync(status);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
            
        }

        [Authorize(Roles = $"{Roles.Manager}")]
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(string id)
        {
            try
            {
                await _service.ApproveAsync(id);
                return Ok(new { message = "Заявку прийнято, акаунт створено" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = $"{Roles.Manager}")]
        [HttpPost("{id}/reject")]
        public async Task<IActionResult> Reject(string id)
        {
            try
            {
                await _service.RejectAsync(id);
                return Ok(new { message = "Заявку відхилено" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}