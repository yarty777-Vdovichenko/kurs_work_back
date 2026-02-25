using kurswork_back.Models;
using kurswork_back.Services;
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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tarifs = await _service.GetAllAsync();

            return Ok(tarifs);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var tarif = await _service.GetByIdAsync(id);

            if (tarif == null)
                return NotFound();

            return Ok(tarif);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Tarif tarif)
        {
            await _service.CreateAsync(tarif);

            return Created();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _service.DeleteAsync(id);

            return Ok();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Tarif tarif)
        {
            var updated = await _service.UpdateAsync(id, tarif);

            if (!updated)
                return NotFound();

            return NoContent();
        }
    }
}
