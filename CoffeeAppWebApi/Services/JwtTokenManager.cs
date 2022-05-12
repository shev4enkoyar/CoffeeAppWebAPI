using CoffeeAppWebApi.DataEf;
using CoffeeAppWebApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace CoffeeAppWebApi.Services
{
    public class JwtTokenManager : IJwtTokenManager
    {
        readonly IConfiguration _configuration;
        readonly ApplicationDbContext _context;
        public JwtTokenManager(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public string Authenticate(string userName, string password)
        {
            var user = _context.Users.SingleOrDefault(x => x.Username.Equals(userName) && x.Password.Equals(password));
            if (user == null)
            {
                return null;
            }
            var key = _configuration.GetValue<string>("JwtConfig:Key");
            var keyBytes = Encoding.ASCII.GetBytes(key);

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userName),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddYears(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
            _context.Update(user);
            _context.SaveChanges();
            return tokenHandler.WriteToken(token);
        }

        public string Decode(string jwt, string decodeAttribute)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(jwt);
            var tokenS = jsonToken as JwtSecurityToken;

            return tokenS.Claims.First(claim => claim.Type == decodeAttribute).Value;
        }

        public string Register(User user)
        {
            var userExist = _context.Users.SingleOrDefault(x => x.Username.Equals(user.Username));
            if (userExist != null)
            {
                return null;
            }
            user.Role = "user";
            _context.Add(user);
            _context.SaveChanges();

            var response = Authenticate(user.Username, user.Password);

            return response;
        }
    }
}
