using Microsoft.AspNetCore.Mvc;
using MSIT155Site.Models;

namespace MSIT155Site.Controllers
{
    public class ApiController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        private readonly MyDBContext _context;
        public ApiController(MyDBContext context)
        {
            _context = context;
        }

        public IActionResult Cities()
        {
            var cities = _context.Addresses.Select(p => p.City).Distinct();
            return Json(cities);
        }


        public IActionResult Card()
        {
            return View();
        }


    }
}
