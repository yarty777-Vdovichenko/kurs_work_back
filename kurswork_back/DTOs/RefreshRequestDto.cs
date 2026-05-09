using System.ComponentModel.DataAnnotations;

namespace kurswork_back.DTOs
{
    public class RefreshRequestDto
    {
        [Required(ErrorMessage = "Refresh token обов'язковий")]
        public string RefreshToken { get; set; }
    }
}
