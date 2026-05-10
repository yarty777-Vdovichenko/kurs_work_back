using kurswork_back.DTOs;
using kurswork_back.Models;
using kurswork_back.Repositories;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
namespace kurswork_back.Services
{
    public interface IRegistrationRequestService
    {
        Task CreateRequestAsync(RegisterDto dto);
        Task<List<RegistrationRequestDto>> GetAllAsync(string? status);
        Task ApproveAsync(string id);
        Task RejectAsync(string id);
    }

    public class RegistrationRequestService : IRegistrationRequestService
    {
        private readonly IRegistrationRequestRepository _requestRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public RegistrationRequestService(
            IRegistrationRequestRepository requestRepository,
            IUserRepository userRepository,
            IPasswordHasher passwordHasher)
        {
            _requestRepository = requestRepository;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task CreateRequestAsync(RegisterDto dto)
        {

            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new Exception("Email вже використовується");

            var existingRequest = await _requestRepository.GetPendingByEmailAsync(dto.Email);
            if (existingRequest != null)
                throw new Exception("Заявка вже надіслана, очікуйте підтвердження");

            var request = new RegistrationRequest
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = _passwordHasher.HashPassword(dto.Password)
            };

            await _requestRepository.CreateAsync(request);
        }

        public async Task<List<RegistrationRequestDto>> GetAllAsync(string? status)
        {
            var requests = await _requestRepository.GetAllAsync();

            if (requests.Count == 0)
                throw new Exception("Нічого не знайдено :(");

            return requests.Select(r => new RegistrationRequestDto
            {
                Id = r.Id!,
                Name = r.Name,
                Email = r.Email,
                Status = r.Status,
                CreatedAt = r.CreatedAt
            }).ToList();
        }

        public async Task ApproveAsync(string id)
        {
            var request = await _requestRepository.GetByIdAsync(id);
            if (request == null)
                throw new Exception("Заявку не знайдено");
            if (request.Status != "Pending")
                throw new Exception("Заявка вже оброблена");

            await SendEmailAsync(request.Email, "Заявку схвалено", "Вітаємо! Вашу заявку схвалено, можете входити.");

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = request.PasswordHash,
                Role = Roles.User
            };

            await _userRepository.CreateAsync(user);
            await _requestRepository.DeleteAsync(id);
        }

        public async Task RejectAsync(string id)
        {
            var request = await _requestRepository.GetByIdAsync(id);
            if (request == null)
                throw new Exception("Заявку не знайдено");
            if (request.Status != "Pending")
                throw new Exception("Заявка вже оброблена");

            await SendEmailAsync(request.Email, "Заявку відхилено", "На жаль, вашу заявку відхилено.");

            await _requestRepository.DeleteAsync(id);
        }
        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse("yaroslav0908l@gmail.com"));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new BodyBuilder { TextBody = body }.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync("yaroslav0908l@gmail.com", "w t c i n i v f v e e k r y u b");
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }
    }
}