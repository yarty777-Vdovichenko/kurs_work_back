using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace kurswork_back.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        [Required(ErrorMessage = "Ім'я обов'язкове")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Ім'я має бути від 2 до 100 символів")]
        public string Name { get; set; }

        [BsonElement("email")]
        [Required(ErrorMessage = "Email обов'язковий")]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        [StringLength(254, ErrorMessage = "Email не може бути довшим за 254 символи")]
        public string Email { get; set; }

        [BsonElement("passwordHash")]
        [Required(ErrorMessage = "Пароль обов'язковий")]
        public string PasswordHash { get; set; }

        [BsonElement("role")]
        [Required(ErrorMessage = "Роль обов'язкова")]
        [RegularExpression("^(User|Admin|Manager)$", ErrorMessage = "Роль має бути: User, Admin або Manager")]
        public string Role { get; set; }

        [BsonElement("refreshToken")]
        public string? RefreshToken { get; set; }

        [BsonElement("refreshTokenExpiryTime")]
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
