using System.ComponentModel.DataAnnotations;

namespace kurswork_back.DTOs
{
    public class UpdateSimDto
    {
        [Required(ErrorMessage = "Статус обов'язковий")]
        [RegularExpression("^(active|inactive|blocked)$", ErrorMessage = "Статус має бути: active, inactive або blocked")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Тариф обов'язковий")]
        [RegularExpression("^[a-fA-F0-9]{24}$", ErrorMessage = "Невірний формат TarifId")]
        public string TarifId { get; set; }
    }
}
