using System.ComponentModel.DataAnnotations;

namespace CoffeeAppWebApi.Helpers
{
    public class AuthenticateRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
