using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace kurswork_back.Models
{
    public class SimCard
    {
        [BsonElement("id")]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("SimNumber")]
        [RegularExpression(@"^\+?[0-9]{10,15}$", ErrorMessage = "Номер SIM має містити від 10 до 15 цифр")]
        public string? SimNumber { get; set; }

        [BsonElement("Status")]
        [Required(ErrorMessage = "Статус обов'язковий")]
        [RegularExpression("^(active|inactive|blocked)$", ErrorMessage = "Статус має бути: active, inactive або blocked")]
        public string? Status { get; set; } = "active";

        [BsonElement("CreatedAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; }

        [BsonElement("TarifId")]
        [Required(ErrorMessage = "Тариф обов'язковий")]
        public string TarifId { get; set; }
    }
}
