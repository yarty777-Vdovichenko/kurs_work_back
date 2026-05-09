using System.ComponentModel.DataAnnotations;

namespace kurswork_back.DTOs
{
    public class CreateUserDto
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Ім'я обов'язкове")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Ім'я має бути від 2 до 100 символів")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email обов'язковий")]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        [StringLength(254, ErrorMessage = "Email не може бути довшим за 254 символи")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль обов'язковий")]
        [MinLength(6, ErrorMessage = "Пароль має бути не менше 6 символів")]
        [MaxLength(100, ErrorMessage = "Пароль не може бути довшим за 100 символів")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Роль обов'язкова")]
        [RegularExpression("^(User|Admin|Manager)$", ErrorMessage = "Роль має бути: User, Admin або Manager")]
        public string Role { get; set; }
    }
}
