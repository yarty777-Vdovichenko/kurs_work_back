using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace kurswork_back.Models
{
    public class RegistrationRequest
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = "Pending";

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("reviewedBy")]
        public string? ReviewedBy { get; set; }

        [BsonElement("reviewedAt")]
        public DateTime? ReviewedAt { get; set; }
    }
}
