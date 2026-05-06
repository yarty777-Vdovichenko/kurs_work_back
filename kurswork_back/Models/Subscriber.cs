using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.CompilerServices;

namespace kurswork_back.Models
{
    public class Subscriber
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonElement("name")]
        public string FullName { get; set; }
        [BsonElement("createdat")]
        public DateTime CreatedAt { get; set; }
        [BsonElement("sims")]
        public List<SimCard> Sims { get; set; }
    }
}
