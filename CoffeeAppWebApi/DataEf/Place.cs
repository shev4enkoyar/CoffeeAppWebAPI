using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CoffeeAppWebApi.DataEf
{
    [Index(nameof(Latitude), nameof(Longitude), IsUnique = true, Name = "Location")]
    public class Place
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        public double Rating { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
        public string Image { get; set; }

        [JsonIgnore]
        public string Details { get; set; }
        [JsonIgnore]
        public List<Favorite> Favorites { get; set; } = new List<Favorite>();
        [JsonIgnore]
        public List<AmountUsedCoffee> AmountUsedCoffee { get; set; } = new List<AmountUsedCoffee>();
    }
}
