using kurswork_back.DTOs;
using kurswork_back.Models;
using kurswork_back.Repositories;

namespace kurswork_back.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<AuthResponseDto> RefreshAsync(string refreshToken);
        Task LogoutAsync(string userId);
    }

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(IUserRepository userRepository, IJwtService jwtService, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new Exception("Користувач вже існує");
            if (!dto.Email.Contains("@")|| !dto.Email.Contains("."))
            {
                throw new Exception("Неправильний мейл");
            }
            if (dto.Name.Length < 5)
            {
                throw new Exception("Коротке ім'я");
            }
            if (dto.Password.Length < 6)
            {
                throw new Exception("Короткий пароль");
            }
            if(dto.Role!="Meneger"&&dto.Role!="Admin"&&dto.Role!="User")
            {
                throw new Exception("Не чітери)");
            }
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = _passwordHasher.HashPassword(dto.Password),
                Role = dto.Role
            };

            await _userRepository.CreateAsync(user);

            return await GenerateTokensAsync(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null)
                throw new Exception("Неправильний пароль або ел. пошта");

            var isValid = _passwordHasher.Verify(dto.Password, user.PasswordHash);
            if (!isValid)
                throw new Exception("Неправильний пароль або ел. пошта");

            return await GenerateTokensAsync(user);
        }

        public async Task<AuthResponseDto> RefreshAsync(string refreshToken)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
            if (user == null)
                throw new Exception("Invalid refresh token");

            if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new Exception("Refresh token expired");

            return await GenerateTokensAsync(user);
        }

        public async Task LogoutAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            await _userRepository.UpdateAsync(user);
        }

        private async Task<AuthResponseDto> GenerateTokensAsync(User user)
        {
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userRepository.UpdateAsync(user);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = new UserDto
                {
                    Id = user.Id!,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role
                }
            };
        }
    }
}