using kurswork_back.DTOs;
using kurswork_back.Models;
using kurswork_back.Repositories;
using System.Data;

namespace kurswork_back.Services
{
        public interface IUserService
        {
            Task<List<User>> GetAllAsync();
            Task<User?> GetByIdAsync(string id);
            Task CreateAsync(CreateUserDto user);
            Task DeleteAsync(string id);
            Task<bool> UpdateAsync(string id, CreateUserDto user);
            Task<bool> PatchAsync(string id, UpdateUserDto dto);
        }
    
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IPasswordHasher _passwordHasher;
        public UserService(IUserRepository repository,IPasswordHasher hasher)
        {
            _repository = repository;
            _passwordHasher = hasher;
        }
        
        public async Task<List<User>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<User?> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Немає ід");

            return await _repository.GetByIdAsync(id);
        }
        public async Task CreateAsync(CreateUserDto user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(user.Email))
                throw new Exception("Ел. пошта обов'язкова");

            var existingUser = await _repository.GetByEmailAsync(user.Email);

            if (user.Password.Length < 6)
                throw new Exception("Короткий пароль!");

            if (existingUser != null)
                throw new Exception("Ел. пошта вже зайнята");

            if (string.IsNullOrWhiteSpace(user.Name))
                throw new Exception("Ім'я обов'язкове");

            if (user.Role != "Manager" && user.Role != "Admin" && user.Role != "User")
                throw new Exception("Є тільки 3 ролі:User,Meneger,Admin");
            
            var userOK = new User
            {
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                PasswordHash = _passwordHasher.HashPassword(user.Password)
            };

            await _repository.CreateAsync(userOK);
        }
        public async Task DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Пустий ід");

            await _repository.DeleteAsync(id);
        }
        public async Task<bool> UpdateAsync(string id, CreateUserDto userDto)
        {
            var oldUser = await GetByIdAsync(id);
            if (oldUser == null)
                return false;

            oldUser.Name = userDto.Name;
            oldUser.Email = userDto.Email;
            if (!string.IsNullOrEmpty(userDto.Password))
                oldUser.PasswordHash = _passwordHasher.HashPassword(userDto.Password);

            oldUser.Role = userDto.Role;

            await _repository.UpdateAsync(oldUser);
            return true;
        }
        public async Task<bool> PatchAsync(string id, UpdateUserDto dto)
        {
            var user = await GetByIdAsync(id);
            if (user == null)
                return false;

            if (!string.IsNullOrWhiteSpace(dto.Name))
                user.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var existing = await _repository.GetByEmailAsync(dto.Email);
                if (existing != null && existing.Id != id)
                    throw new Exception("Ел. пошта вже зайнята");
                user.Email = dto.Email;
            }

            if (!string.IsNullOrWhiteSpace(dto.Password))
                user.PasswordHash = _passwordHasher.HashPassword(dto.Password);

            if (!string.IsNullOrWhiteSpace(dto.Role))
            {
                if (dto.Role != "Manager" && dto.Role != "Admin" && dto.Role != "User")
                    throw new Exception("Є тільки 3 ролі: User, Meneger, Admin");
                user.Role = dto.Role;
            }

            await _repository.UpdateAsync(user);
            return true;
        }
    }
}
