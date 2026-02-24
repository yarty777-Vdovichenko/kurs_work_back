using kurswork_back.Models;
using kurswork_back.Services;
using kurswork_back.Services;
using Microsoft.AspNetCore.Mvc;

namespace kurswork_back.Controllers
{
    // Каже ASP.NET:
    // "Це контролер, тут буде HTTP"
    [ApiController]

    // Базовий URL для цього контролера
    // api/users
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        // Контролер знає ТІЛЬКИ про Service
        private readonly IUserService _userService;

        // Service прийде через DI
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // ============================
        // GET api/users
        // ============================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Викликаємо сервіс
            var users = await _userService.GetAllAsync();

            // Повертаємо 200 OK + JSON
            return Ok(users);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userService.GetByIdAsync(id);

            // Якщо користувача не знайдено
            if (user == null)
                return NotFound(); // 404

            return Ok(user); // 200 + JSON
        }
        // ============================
        // POST api/users
        // ============================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            // FromBody = JSON з body запиту
            await _userService.CreateAsync(user);

            // 200 OK (пізніше можна 201 Created)
            return Ok();
        }
        // ============================
        // DELETE api/users/{id}
        // ============================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _userService.DeleteAsync(id);
            return NoContent(); // 204
        }

    }
}