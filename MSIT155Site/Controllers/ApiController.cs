﻿using Microsoft.AspNetCore.Mvc;
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
            return Content("<h2>Hello 你好!!</h2>", "text/html", System.Text.Encoding.UTF8);
        }


        private readonly MyDBContext _context;
        private readonly IWebHostEnvironment _environment;

        public ApiController(MyDBContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult Cities()
        {
            var cities = _context.Addresses.Select(p => p.City).Distinct();
            return Json(cities);
        }

        public IActionResult Districts(string city)
        {
            var districts = _context.Addresses.Where(a => a.City == city).Select(c => c.SiteId).Distinct();

            return Json(districts);
        }

        public IActionResult Roads(string siteId)
        {
            var roads = _context.Addresses.Where(a => a.SiteId == siteId).Select(c => c.Road).Distinct();

            return Json(roads);
        }


        public IActionResult Address()
        {
            return View();
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

        public IActionResult Show()
        {
            return View();
        }


        public IActionResult First()
        {
            return View();
        }
        public IActionResult CheckAccount(UserDTO user)
        {

            bool isDuplicate = _context.Members.Any(p => p.Name == user.Name);

            return Content($"{isDuplicate}", "text/plain", Encoding.UTF8);
        }

        [HttpPost]
        public IActionResult Register(Member _user, IFormFile Avatar)
        {
            if (string.IsNullOrEmpty(_user.Name))
            {
                _user.Name = "guest";
            }
            //todo
            //1. 只允許上傳圖檔
            //2. 圖檔最大2M
            //3. 檔案名稱重複處理

            //string uploadPath = @"C:\Shared\AjaxWorkspace\MSIT155Site\wwwroot\uploads\a.jpg";
            string fileName = "empty.jpg";
            if (Avatar != null)
            {
                fileName = Avatar.FileName;
            }
            string uploadPath = Path.Combine(_environment.WebRootPath, "uploads", fileName);

            using (var fileStream = new FileStream(uploadPath, FileMode.Create))
            {
                Avatar?.CopyTo(fileStream);
            }

            // return Content($"Hello {_user.Name}, {_user.Age}歲了, 電子郵件是 {_user.Email}","text/plain", Encoding.UTF8);
            //return Content($"{_user.Avatar?.FileName} - {_user.Avatar?.ContentType} - {_user.Avatar?.Length}");

            //新增到資料庫
            _user.FileName = fileName;
            //轉成二進位
            byte[]? imgByte = null;
            using (var memoryStream = new MemoryStream())
            {
                Avatar?.CopyTo(memoryStream);
                imgByte = memoryStream.ToArray();
            }
            _user.FileData = imgByte;


            _context.Members.Add(_user);
            _context.SaveChanges();


            return Content(uploadPath);
        }


        //景點資料
        [HttpPost]
        public IActionResult Spots([FromBody] SearchDTO _search)
        {
            //根據分類編號搜尋
            var spots = _search.CategoryId == 0 ? _context.SpotImagesSpots : _context.SpotImagesSpots.Where(s => s.CategoryId == _search.CategoryId);

            //根據關鍵字搜尋
            if (!string.IsNullOrEmpty(_search.Keyword))
            {
                spots = spots.Where(s => s.SpotTitle.Contains(_search.Keyword) || s.SpotDescription.Contains(_search.Keyword));
            }

            //排序
            switch (_search.SortBy)
            {
                case "spotTitle":
                    spots = _search.SortType == "asc" ? spots.OrderBy(s => s.SpotTitle) : spots.OrderByDescending(s => s.SpotTitle);
                    break;
                case "categoryId":
                    spots = _search.SortType == "asc" ? spots.OrderBy(s => s.CategoryId) : spots.OrderByDescending(s => s.CategoryId);
                    break;
                default: //spotId
                    spots = _search.SortType == "asc" ? spots.OrderBy(s => s.SpotId) : spots.OrderByDescending(s => s.SpotId);
                    break;
            }

            //總共有幾筆
            int totalCount = spots.Count();
            //一頁幾筆資料
            int pageSize = _search.PageSize ?? 9;
            //計算總共有幾頁
            int totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            //目前第幾頁
            int page = _search.Page ?? 1;


            //分頁 Skip=跳過N筆資料、Take=取得N筆資料
            spots = spots.Skip((page - 1) * pageSize).Take(pageSize);

            //裝入DTO中
            SpotsPagingDTO spotsPaging = new SpotsPagingDTO();
            spotsPaging.TotalPages = totalPages;
            spotsPaging.TotalCount = totalCount;
            spotsPaging.SpotsResult = spots.ToList();
            return Json(spotsPaging);

        }



        public IActionResult SpotTitle(string title)
        {
            var titles = _context.Spots.Where(s => s.SpotTitle.Contains(title)).Select(s => s.SpotTitle).Take(8);
            return Json(titles);
        }

    }
}
