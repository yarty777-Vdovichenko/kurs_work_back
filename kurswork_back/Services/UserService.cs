using kurswork_back.Repositories;
using kurswork_back.Models;

namespace kurswork_back.Services
{
        public interface IUserService
        {
            // Отримати всіх користувачів
            Task<List<User>> GetAllAsync();

            // Отримати користувача по Id
            Task<User?> GetByIdAsync(string id);

            // Створити користувача
            Task CreateAsync(User user);

            // Видалити користувача
            Task DeleteAsync(string id);
        }
    
    public class UserService : IUserService
    {
        // Сервіс НЕ працює з Mongo напряму
        private readonly IUserRepository _repository;

        // Repository прийде через DI
        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        // Отримати всіх користувачів
        public async Task<List<User>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        // Отримати користувача по Id
        public async Task<User?> GetByIdAsync(string id)
        {
            // ЛОГІКА: перевірка Id
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Id is empty");

            return await _repository.GetByIdAsync(id);
        }

        // Створити користувача
        public async Task CreateAsync(User user)
        {
            // БІЗНЕС-ЛОГІКА ↓↓↓

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(user.Email))
                throw new Exception("Email is required");

            if (string.IsNullOrWhiteSpace(user.Name))
                throw new Exception("Name is required");

            if (user.Role != "Meneger" || user.Role != "Operator" || user.Role != "Admin")
                throw new Exception("We have only three roles:Operator,Meneger,Admin");

            // Якщо всі перевірки пройшли —
            // дозволяємо репозиторію зберегти
            await _repository.CreateAsync(user);
        }

        // Видалити користувача
        public async Task DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Id is empty");

            await _repository.DeleteAsync(id);
        }
    }
}
