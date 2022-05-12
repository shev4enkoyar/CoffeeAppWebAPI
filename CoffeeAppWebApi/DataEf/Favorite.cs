using System.Text.Json.Serialization;

namespace CoffeeAppWebApi.DataEf
{
    public class Favorite
    {
        public int Id { get; set; }

        public int PlaceId { get; set; }
        [JsonIgnore]
        public Place Place { get; set; }

        public int UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }
    }
}
