using CoffeeAppWebApi.DataEf;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
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
            if (user.IsActive == false)
            {
                if (SendEmail(user))
                {
                    return $"activateAccount {tokenHandler.WriteToken(token)}";
                }
                return null;
                
            }
            return tokenHandler.WriteToken(token);
        }

        private bool SendEmail(User user)
        {
            EmailService emailService = new(_configuration);
            string htmlTemplate = $"<html><body><div style=\"width: 600px\"><div style=\"padding-top:24px;font-weight:bold;color:#000000;font-size:14px\">Уважаемый {user.Name},</div>" +
                $"<div style=\"padding-top:24px;color:#000000;font-size:12px\">Спасибо за проявленный интерес к кофе и нашему приложению.</div>" +
                $"<hr style=\"height:1px;background-color:#c0c0c0;color:#c0c0c0\">" +
                $"<div style=\"padding-top:20px;padding-bottom:16px;color:#000000;font-size:12px\">Вы получили данное сообщение, так как пытались активировать аккаунт. Для завершения верификации аккаунта, введите код находящийся ниже</div>" +
                $"<div style=\"padding:12px;background-color:#e0e0e0;color:#000000;font-weight:bold;font-size:16px\" align=\"center\">{GenUniqNumber(user.Id)}</div></div></body></html>";
            var response = emailService.SendEmail(user.Email, "Верификация аккаунта", htmlTemplate);
            return response;
        }

        private int GenUniqNumber(int id)
        {
            int result = 0;
            bool isUniq = false;
            while (!isUniq)
            {
                List<int> numbers = new List<int>();
                for (int i = 0; i < 6; i++)
                    numbers.Add(RandomNumberGenerator.GetInt32(0, 10));
                if (!_context.UserVerificationCodes.Any(x => x.Code == int.Parse(string.Join("", numbers.ToArray()))))
                {
                    isUniq = true;
                    result = int.Parse(string.Join("", numbers.ToArray()));
                }
            }
            _context.UserVerificationCodes.Add(new UserVerificationCode { UserId = id, Code = result });
            _context.SaveChanges();
            return result;
        }

        public string Decode(string jwt, string decodeAttribute)
        {
            if (jwt.Contains(' '))
            {
                jwt = jwt.Split(' ')[1];
            }
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
