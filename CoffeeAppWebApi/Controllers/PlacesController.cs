using CoffeeAppWebApi.DataEf;
using CoffeeAppWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;

namespace CoffeeAppWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlacesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PlacesController(ApplicationDbContext context)
        {
            _context = context;
        }

        //TEST GetPins
        [AllowAnonymous]
        [HttpGet("getpins")]
        public IActionResult GetPins()
        {
            return Ok(_context.Places);
        }

        //TEST SearchByName
        [AllowAnonymous]
        [HttpGet("searchbyname/{name}")]
        public IActionResult SearchByName(string name)
        {
            if (name == null)
                return BadRequest();
            var results = _context.Places.Where(f => EF.Functions.FreeText(f.Name, name));
            return Ok(results);
        }

        //TEST GetInfoById
        [AllowAnonymous]
        [HttpGet("getinfobyid/{id}")]
        public IActionResult GetInfoById(int id)
        {
            return Ok(_context.Places.Where(x => x.Id == id));
        }

        [AllowAnonymous]
        [HttpGet("getdetailsbyid/{id}")]
        public IActionResult GetDetailsById(int id)
        {
            var query = _context.Places.FirstOrDefault(x => x.Id == id);
            if (query == null)
                return NotFound();
            return Ok(new PlaceSubInfo() { Details = query.Details });
        }

        [AllowAnonymous]
        [HttpGet("image/{folder}/{name}")]
        public IActionResult Image(string folder, string name)
        {
            string directory = Directory.GetCurrentDirectory();
            string path = Path.Combine(directory, "Images", folder, name);
            if (System.IO.File.Exists(path))
                return PhysicalFile(path, "image/jpeg");
            return NotFound($"Image not found on path: {path}");

        }
    }
}
