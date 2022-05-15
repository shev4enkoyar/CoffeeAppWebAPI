using CoffeeAppWebApi.DataEf;
using CoffeeAppWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace CoffeeAppWebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VerificationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtTokenManager _tokenManager;
        private IConfiguration Configuration { get; }

        public VerificationController(IConfiguration configuration, IJwtTokenManager tokenManager, ApplicationDbContext context)
        {
            Configuration = configuration;
            _tokenManager = tokenManager;
            _context = context;
        }

        [Authorize(Roles = "user, admin")]
        [HttpGet("sendemail")]
        public IActionResult SendEmail([FromHeader] string authorization)
        {
            var login = _tokenManager.Decode(authorization, "nameid");
            var user = _context.Users.FirstOrDefault(x => x.Username == login);
            EmailService emailService = new(Configuration);
            string htmlTeplate = $"<html><body><div style=\"width: 600px\"><div style=\"padding-top:24px;font-weight:bold;color:#000000;font-size:14px\">Уважаемый {user.Name},</div>" +
                $"<div style=\"padding-top:24px;color:#000000;font-size:12px\">Спасибо за проявленный интерес к кофе и нашему приложению.</div>" +
                $"<hr style=\"height:1px;background-color:#c0c0c0;color:#c0c0c0\">" +
                $"<div style=\"padding-top:20px;padding-bottom:16px;color:#000000;font-size:12px\">Вы получили данное сообщение, так как пытались активировать аккаунт. Для завершения верификации аккаунта, введите код находящийся ниже</div>" +
                $"<div style=\"padding:12px;background-color:#e0e0e0;color:#000000;font-weight:bold;font-size:16px\" align=\"center\">{GenUniqNumber(user.Id)}</div></div></body></html>";
            var response = emailService.SendEmail(user.Email, "Верификация аккаунта", htmlTeplate);
            if (response == false)
                return BadRequest();
            return Ok();
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
            _context.UserVerificationCodes.Add(new UserVerificationCode { UserId = id, Code = result, ExpirationDate = DateTime.Now.AddMinutes(15) });
            _context.SaveChanges();
            return result;
        }

        [Authorize(Roles = "user, admin")]
        [HttpGet("checkcode/{userVerificationCode}")]
        public IActionResult CheckCode([FromHeader] string authorization, string userVerificationCode)
        {
            var user = _context.Users.FirstOrDefault(x => x.Username == _tokenManager.Decode(authorization, "nameid"));
            var verificationCode = _context.UserVerificationCodes.FirstOrDefault(x => x.Code == int.Parse(userVerificationCode) && x.UserId == user.Id);
            if (verificationCode == null)
                return BadRequest();
            user.IsActive = true;
            _context.Update(user);
            _context.Remove(verificationCode);
            _context.SaveChanges();
            return Ok();
        }
    }
}
