using CoffeeAppWebApi.DataEf;

namespace CoffeeAppWebApi.Services
{
    public interface IJwtTokenManager
    {
        string Authenticate(string userName, string password);
        string Register(User user);
        string Decode(string jwt, string decodeAttribute);
    }
}
