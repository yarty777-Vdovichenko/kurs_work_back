using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace kurswork_back.Models
{
    public class Subscriber
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        [Required(ErrorMessage = "Повне ім'я обов'язкове")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Повне ім'я має бути від 2 до 150 символів")]
        [RegularExpression(@"^\S+\s\S+\s\S+$", ErrorMessage = "Введіть ПІБ (3 слова через пробіл)")]
        public string FullName { get; set; }

        [BsonElement("createdat")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; }

        [BsonElement("sims")]
        [Required(ErrorMessage = "Список SIM-карт обов'язковий")]
        public List<SimCard> Sims { get; set; } = [];
    }
}
