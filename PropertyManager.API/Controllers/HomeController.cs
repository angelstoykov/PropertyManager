using Microsoft.AspNetCore.Mvc;
using PropertyManager.Data;

namespace PropertyManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly PropertyManagerDbContext _context;

        public HomeController(PropertyManagerDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(new { name = "pesho" });
        }
    }
}
