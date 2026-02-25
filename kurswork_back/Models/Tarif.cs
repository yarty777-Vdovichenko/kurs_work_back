using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace kurswork_back.Models
{    
    public class Tarif
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("internet_capacity")]
        public double Internet_capacity { get; set; }
        [BsonElement("minutes")]
        public int Minutes { get; set; }
        [BsonElement("additional")]
        public string Additional { get; set; }
        [BsonElement("price")]
        public double Price { get; set; }
    }
}
