using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MSIT155Site.Models;
using MSIT155Site.Models.DTO;
using System.Text;

namespace MSIT155Site.Controllers
{
    public class ApiController : Controller
    {
        public IActionResult Index()
        {
            Thread.Sleep(3000);
            //int a = 10;
            //int b = 0;
            //int c = a / b;
            return Content("<h2>Hello 你好!!</h2>", "text/html",System.Text.Encoding.UTF8);
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

        public IActionResult Avatar(int id = 1)
        {
            Member? member = _context.Members.Find(id);
            if (member != null)
            {
                byte[] img = member.FileData;
                if (img != null)
                {
                    return File(img, "image/jpeg");
                }
            }
            return NotFound();
        }

        public IActionResult First()
        {
     
            return View();
        }
        public IActionResult CheckAccount()
        {
            return View();
        }
        
        public IActionResult Register(UserDTO user)
        {
            if (string.IsNullOrEmpty(user.Name))
            {
                user.Name = "guest";
            }
            return Content($"Hello {user.Name}, {user.Age}歲了", "text/plain", Encoding.UTF8);
        }


    }
}
