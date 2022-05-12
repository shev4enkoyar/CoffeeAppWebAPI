using System;
using System.Text.Json.Serialization;

namespace CoffeeAppWebApi.DataEf
{
    public class AmountUsedCoffee
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }
        public int PlaceId { get; set; }
        [JsonIgnore]
        public Place Place { get; set; }
        public int CoffeeListId { get; set; }
        public CoffeeList CoffeeList { get; set; }
        public DateTime Date { get; set; }
    }
}
