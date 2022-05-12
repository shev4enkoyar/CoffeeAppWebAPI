using CoffeeAppWebApi.DataEf;
using CoffeeAppWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeAppWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IJwtTokenManager _tokenManager;
        public TokenController(IJwtTokenManager tokenManager)
        {
            _tokenManager = tokenManager;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] User user)
        {
            var token = _tokenManager.Authenticate(user.Username, user.Password);
            if (string.IsNullOrEmpty(token))
                return Unauthorized();
            return Ok(token);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            var response = _tokenManager.Register(user);

            if (response == null)
            {
                return BadRequest("Didn't register!");
            }

            return Ok(response);
        }
    }
}
