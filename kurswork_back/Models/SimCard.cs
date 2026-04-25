using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace kurswork_back.Models
{
    public class SimCard
    {
        [BsonElement("id")]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        [BsonElement("SimNumber")]
        public string SimNumber { get; set; }
        [BsonElement("Status")]
        public string Status { get; set; }
        [BsonElement("CreatedAt")]
        public string CreatedAt { get; set; }
        [BsonElement("TarifId")]
        public string TarifId { get; set; }
    }
}