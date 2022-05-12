using CoffeeAppWebApi.DataEf;
using CoffeeAppWebApi.Helpers;
using CoffeeAppWebApi.Models;
using CoffeeAppWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace CoffeeAppWebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtTokenManager _tokenManager;

        public UsersController(ApplicationDbContext context, IJwtTokenManager tokenManager)
        {
            _context = context;
            _tokenManager = tokenManager;
        }

        [Authorize(Roles = "user, admin")]
        [HttpGet("getinfo")]
        public IActionResult GetInfo([FromHeader] string authorization)
        {
            string jwt = authorization.Split(' ')[1];
            var login = _tokenManager.Decode(jwt, "nameid");
            var userInfo = _context.Users.FirstOrDefault(x => x.Username == login).WithoutPrivateInfo();
            if (userInfo == null)
                return NotFound();
            return Ok(userInfo);
        }

        [Authorize(Roles = "user, admin")]
        [HttpGet("getfavorites")]
        public IActionResult GetFavorites([FromHeader] string authorization)
        {
            string jwt = authorization.Split(' ')[1];
            var login = _tokenManager.Decode(jwt, "nameid");
            var favorites = _context.Favorites.FirstOrDefault(x => x.User.Username == login);
            if (favorites == null)
                return NotFound();
            return Ok(favorites);
        }
    }
}
