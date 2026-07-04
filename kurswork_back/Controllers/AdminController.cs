using kurswork_back.Data;
using kurswork_back.Infrastructure.Seed;
using Microsoft.AspNetCore.Mvc;

namespace kurswork_back.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IDatabaseSeeder _seeder;
        private readonly IConfiguration _config;

        public AdminController(IDatabaseSeeder seeder, IConfiguration config)
        {
            _seeder = seeder;
            _config = config;
        }

        [HttpPost("reset-demo")]
        public async Task<IActionResult> ResetDemo([FromHeader(Name = "X-Reset-Secret")] string secret)
        {
            var expected = _config["Demo:ResetSecret"];
            if (string.IsNullOrEmpty(expected) || secret != expected)
                return Unauthorized();

            await _seeder.ResetDemoDataAsync();
            return Ok(new { message = "Demo data reset" });
        }
    }
}