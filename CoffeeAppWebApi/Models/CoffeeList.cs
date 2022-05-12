using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CoffeeAppWebApi.Models
{
    public class CoffeeList
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public List<AmountUsedCoffee> AmountUsedCoffee { get; set; } = new List<AmountUsedCoffee>();
    }
}
