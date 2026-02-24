using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace kurswork_back.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; } = null;

        [BsonElement("name")]
        public string Name { get; set; }


        [BsonElement("email")]
        public string Email { get; set; }


        [BsonElement("role")]
        public string Role { get; set; }

    }
}