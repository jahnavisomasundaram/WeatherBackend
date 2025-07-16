using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WeatherBackend.Models
{
    public class RegisterData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }

        public string Email { get; set; }

        public string ConfirmPassword { get; set; }

        public List<string> Favourites { get; set; } = new();
    }
}
