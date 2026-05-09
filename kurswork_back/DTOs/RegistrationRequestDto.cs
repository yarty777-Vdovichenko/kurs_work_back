using System.ComponentModel.DataAnnotations;

namespace kurswork_back.DTOs
{
    public class RegistrationRequestDto
    {
        [Required]
        public string Id { get; set; }

        [Required(ErrorMessage = "Ім'я обов'язкове")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Ім'я має бути від 2 до 100 символів")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email обов'язковий")]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Статус обов'язковий")]
        [RegularExpression("^(Pending|Approved|Rejected)$", ErrorMessage = "Статус має бути: Pending, Approved або Rejected")]
        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
