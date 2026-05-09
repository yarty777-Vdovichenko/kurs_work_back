using System.ComponentModel.DataAnnotations;

namespace kurswork_back.DTOs
{
    public class CreateSimDto
    {
        [Required(ErrorMessage = "Тариф обов'язковий")]
        [RegularExpression("^[a-fA-F0-9]{24}$", ErrorMessage = "Невірний формат TarifId")]
        public string TarifId { get; set; }
    }
}
