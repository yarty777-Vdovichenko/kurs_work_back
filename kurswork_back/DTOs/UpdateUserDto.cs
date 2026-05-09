using System.ComponentModel.DataAnnotations;

namespace kurswork_back.DTOs
{
    public class UpdateUserDto
    {
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Ім'я має бути від 2 до 100 символів")]
        public string? Name { get; set; }

        [EmailAddress(ErrorMessage = "Невірний формат email")]
        [StringLength(254, ErrorMessage = "Email не може бути довшим за 254 символи")]
        public string? Email { get; set; }

        [MinLength(6, ErrorMessage = "Пароль має бути не менше 6 символів")]
        [MaxLength(100, ErrorMessage = "Пароль не може бути довшим за 100 символів")]
        public string? Password { get; set; }

        [RegularExpression("^(User|Admin|Manager)$", ErrorMessage = "Роль має бути: User, Admin або Manager")]
        public string? Role { get; set; }
    }
}
