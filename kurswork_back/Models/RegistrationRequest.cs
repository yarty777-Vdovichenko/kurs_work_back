using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace kurswork_back.Models
{
    public class RegistrationRequest
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

        [BsonElement("status")]
        [Required(ErrorMessage = "Статус обов'язковий")]
        [RegularExpression("^(Pending|Approved|Rejected)$", ErrorMessage = "Статус має бути: Pending, Approved або Rejected")]
        public string Status { get; set; } = "Pending";

        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("reviewedBy")]
        [StringLength(254, ErrorMessage = "ReviewedBy не може бути довшим за 254 символи")]
        public string? ReviewedBy { get; set; }

        [BsonElement("reviewedAt")]
        public DateTime? ReviewedAt { get; set; }
    }
}
