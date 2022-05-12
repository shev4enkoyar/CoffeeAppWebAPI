using CoffeeAppWebApi.DataEf;
using CoffeeAppWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CoffeeAppWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoffeeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtTokenManager _tokenManager;

        public CoffeeController(ApplicationDbContext context, IJwtTokenManager tokenManager)
        {
            _context = context;
            _tokenManager = tokenManager;
        }

        [AllowAnonymous]
        [HttpGet("getlist")]
        public IActionResult GetList()
        {
            return Ok(_context.CoffeeList);
        }

        [Authorize(Roles = "user, admin")]
        [HttpGet("getusercoffeestats")]
        public IActionResult GetUserCoffeeStats([FromHeader] string authorization)
        {
            string jwt = authorization.Split(' ')[1];
            string login = _tokenManager.Decode(jwt, "nameid");
            int userId = _context.Users.FirstOrDefault(x => x.Username == login).Id;
            var usedCoffee = _context.AmountUsedCoffees.Include(x => x.CoffeeList).Where(x => x.UserId == userId);
            if (usedCoffee.Count() <= 0)
                return NotFound();
            var stats = usedCoffee.GroupBy(x => x.CoffeeList.Name).Select(g => new { Name = g.Key, Count = g.Count() });
            return Ok(stats);
        }

        [AllowAnonymous]
        [HttpGet("getplacecoffeestats/{id}")]
        public IActionResult GetPlaceCoffeeStats(int id)
        {
            var usedCoffee = _context.AmountUsedCoffees.Include(x => x.CoffeeList).Where(x => x.PlaceId == id);
            if (usedCoffee.Count() <= 0)
                return NotFound();
            var stats = usedCoffee.GroupBy(x => x.CoffeeList.Name).Select(g => new { Name = g.Key, Count = g.Count() });
            return Ok(stats);
        }
    }
}
