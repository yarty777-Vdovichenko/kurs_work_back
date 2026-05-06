using kurswork_back.DTOs;
using kurswork_back.Models;
using kurswork_back.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace kurswork_back.Controllers
{
    [ApiController]
    [Route("api/subs")]
    public class SubscriberController : ControllerBase
    {
        private readonly ISubscriberService _service;

        public SubscriberController(ISubscriberService service)
        {
            _service = service;
        }

        [Authorize(Roles = $"{Roles.User},{Roles.Admin},{Roles.Manager}")]
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var result = await _service.GetStatsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = $"{Roles.User},{Roles.Admin},{Roles.Manager}")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1)
        {
            try
            {
                var result = await _service.GetAllAsync(page);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = $"{Roles.User},{Roles.Admin},{Roles.Manager}")]
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string fullName, [FromQuery] string number, [FromQuery] int page = 1)
        {
            try
            {
                var result = await _service.SearchAsync(number, fullName, page);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = $"{Roles.User},{Roles.Admin},{Roles.Manager}")]
        [HttpGet("filter")]
        public async Task<IActionResult> Filter([FromQuery] string? simStatus = null, [FromQuery] string? tarifId = null, [FromQuery] int page = 1)
        {
            try
            {
                var result = await _service.FilterAsync(simStatus, tarifId, page);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = $"{Roles.User},{Roles.Admin},{Roles.Manager}")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var sub = await _service.GetByIdAsync(id);

                if (sub == null)
                    return NotFound();

                return Ok(sub);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = $"{Roles.User},{Roles.Admin},{Roles.Manager}")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Subscriber sub)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _service.CreateAsync(sub);
                return Created();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = $"{Roles.User},{Roles.Admin},{Roles.Manager}")]
        [HttpGet("{id}/sims")]
        public async Task<IActionResult> GetSims(string id)
        {
            try
            {
                var subscriber = await _service.GetByIdAsync(id);

                if (subscriber == null)
                    return NotFound();

                return Ok(subscriber.Sims);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = $"{Roles.User},{Roles.Admin},{Roles.Manager}")]
        [HttpGet("{id}/sims/{simId}")]
        public async Task<IActionResult> GetSim(string id, string simId)
        {
            try
            {
                var sim = await _service.GetSimAsync(id, simId);

                if (sim == null)
                    return NotFound();

                return Ok(sim);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = $"{Roles.User},{Roles.Admin},{Roles.Manager}")]
        [HttpPut("{id}/sims/{simId}")]
        public async Task<IActionResult> UpdateSim(string id, string simId, [FromBody] UpdateSimDto dto)
        {
            try
            {
                var result = await _service.UpdateSimAsync(id, simId, dto);

                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = $"{Roles.User},{Roles.Admin},{Roles.Manager}")]
        [HttpDelete("{id}/sims/{simId}")]
        public async Task<IActionResult> DeleteSim(string id, string simId)
        {
            try
            {
                var result = await _service.DeleteSimAsync(id, simId);

                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = $"{Roles.User},{Roles.Admin},{Roles.Manager}")]
        [HttpPost("{id}/sims")]
        public async Task<IActionResult> AddSim(string id, [FromBody] CreateSimDto dto)
        {
            try
            {
                var result = await _service.AddSimAsync(id, dto);

                if (!result)
                    return NotFound();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = $"{Roles.User},{Roles.Admin},{Roles.Manager}")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = $"{Roles.User},{Roles.Admin},{Roles.Manager}")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Subscriber sub)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updated = await _service.UpdateAsync(id, sub);

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