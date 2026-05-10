using System.ComponentModel.DataAnnotations;

namespace kurswork_back.DTOs
{
    public class CreateSimDto
    {
        [Required(ErrorMessage = "Тариф обов'язковий")]
        public string TarifId { get; set; }
    }
}
