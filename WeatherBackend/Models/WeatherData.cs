using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WeatherBackend.Models
{
    public class WeatherData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string City { get; set; } = string.Empty;
        public double Temperature { get; set; }
        public string Description { get; set; } = string.Empty;

        public int Humidity { get; set; }

        public double Longitude { get; set; }

        public int Sunrise { get; set; }

    }
}
