using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WeatherBackend.Models
{
    public class GoogleData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }


        public string Email { get; set; }

        public List<string> Favourites { get; set; } = new();
    }
}
