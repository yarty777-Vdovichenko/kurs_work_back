using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace kurswork_back.Models 
{
    public class Tarif
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        [Required(ErrorMessage = "Назва обов'язкова")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Назва має бути від 2 до 50 символів")]
        public string Name { get; set; } = string.Empty; 

        [BsonElement("internet_capacity")]
        [Range(0, 1000, ErrorMessage = "Інтернет має бути від 0 до 1000 ГБ")]
        public double Internet_capacity { get; set; }

        [BsonElement("minutes")]
        [Range(0, 10000, ErrorMessage = "Хвилини мають бути від 0 до 10000")]
        public int Minutes { get; set; }

        [BsonElement("additional")]
        [MaxLength(200, ErrorMessage = "Додаткове не більше 200 символів")]
        public string? Additional { get; set; }

        [BsonElement("price")]
        [Range(0.01, 10000.0, ErrorMessage = "Ціна має бути від 0.01 до 10000")]
        public double Price { get; set; }
    }
}