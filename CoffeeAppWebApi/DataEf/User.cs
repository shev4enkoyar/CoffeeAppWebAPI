using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CoffeeAppWebApi.DataEf
{
    [Index(nameof(Username), IsUnique = true)]
    [Index(nameof(PhoneNumber), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        [Required]
        public string Username { get; set; }
        public string Name { get; set; }
        [MaxLength(11)]
        public string PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public bool IsBanned { get; set; }
        public int Age { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }

        [JsonIgnore]
        public List<Favorite> Favorites { get; set; } = new List<Favorite>();
        [JsonIgnore]
        public List<AmountUsedCoffee> AmountUsedCoffee { get; set; } = new List<AmountUsedCoffee>();
    }
}
