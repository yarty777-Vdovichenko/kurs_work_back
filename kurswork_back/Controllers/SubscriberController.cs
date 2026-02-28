using kurswork_back.Models;
using kurswork_back.Services;
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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var subs = await _service.GetAllAsync();

            return Ok(subs);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var sub = await _service.GetByIdAsync(id);

            if (sub == null)
                return NotFound();

            return Ok(sub);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Subscriber sub)
        {
            await _service.CreateAsync(sub);

            return Created();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _service.DeleteAsync(id);

            return Ok();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Subscriber sub)
        {
            var updated = await _service.UpdateAsync(id, sub);

            if (!updated)
                return NotFound();

            return NoContent();
        }
    }
}
