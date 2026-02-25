using kurswork_back.Data;
using kurswork_back.Models;
using MongoDB.Driver;

namespace kurswork_back.Repositories
{
    public interface IUserRepository
    {
        // Отримати всіх користувачів
        Task<List<User>> GetAllAsync();

        // Отримати користувача по Id
        Task<User?> GetByIdAsync(string id);

        // Створити нового користувача
        Task CreateAsync(User user);

        // Видалити користувача
        Task DeleteAsync(string id);
        Task UpdateAsync(User user);
    }
    public class UserRepository : IUserRepository
    {
        // Це наша колекція users
        private readonly IMongoCollection<User> _users;

        // MongoContext прийде через DI
        public UserRepository(MongoContext context)
        {
            // Беремо users з MongoContext
            _users = context.Users;
        }

        // Отримати ВСІХ користувачів
        public async Task<List<User>> GetAllAsync()
        {
            // _ => true означає "без фільтра"
            return await _users
                .Find(_ => true)
                .ToListAsync();
        }

        // Отримати користувача по Id
        public async Task<User?> GetByIdAsync(string id)
        {
            return await _users
                .Find(u => u.Id == id)
                .FirstOrDefaultAsync();
        }

        // Створити користувача
        public async Task CreateAsync(User user)
        {
            await _users.InsertOneAsync(user);
        }

        // Видалити користувача
        public async Task DeleteAsync(string id)
        {
            await _users.DeleteOneAsync(u => u.Id == id);
        }
        public async Task UpdateAsync(User user)
        {
           await _users.ReplaceOneAsync(user => user.Id == user.Id, user);
        }
    }
}
