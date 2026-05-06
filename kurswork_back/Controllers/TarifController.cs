using kurswork_back.Models;
using kurswork_back.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace kurswork_back.Controllers
{
    [ApiController]
    [Route("api/tarifs")]
    public class TarifController : ControllerBase
    {
        private readonly ITarifService _service;

        public TarifController(ITarifService service)
        {
            _service = service;
        }
        [Authorize(Roles = $"{Roles.User},{Roles.Admin},{Roles.Manager}")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var tarifs = await _service.GetAllAsync();

                return Ok(tarifs);
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
                var tarif = await _service.GetByIdAsync(id);

                if (tarif == null)
                    return NotFound();

                return Ok(tarif);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            
        }
        [Authorize(Roles = $"{Roles.User},{Roles.Admin},{Roles.Manager}")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Tarif tarif)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _service.CreateAsync(tarif);

                return Created();
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
        public async Task<IActionResult> Update(string id, [FromBody] Tarif tarif)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var updated = await _service.UpdateAsync(id, tarif);

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
