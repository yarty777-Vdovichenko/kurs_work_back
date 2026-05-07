using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace kurswork_back.Models
{
    public class SimCard
    {
        [BsonElement("id")]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("SimNumber")]
        public string? SimNumber { get; set; }

        [BsonElement("Status")]
        public string? Status { get; set; } = "active";

        [BsonElement("CreatedAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; }

        [BsonElement("TarifId")]
        public string TarifId { get; set; }
    }
}