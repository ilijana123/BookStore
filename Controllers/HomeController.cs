using Microsoft.AspNetCore.Mvc;
using MVCBookk.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using Microsoft.AspNetCore.Http;
namespace MVCBookk.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(IFormFile File)
        {
            if (ModelState.IsValid)
            {
                if (_webHostEnvironment == null)
                {
                    throw new ArgumentNullException(nameof(_webHostEnvironment));
                }
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                
                if (File != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(File.FileName);
                    string fileDir = Path.Combine(wwwRootPath, "images");

                    if (!Directory.Exists(fileDir))
                        Directory.CreateDirectory(fileDir);

                    using (var fileStream = new FileStream(Path.Combine(fileDir, fileName), FileMode.Create))
                    {
                        File.CopyTo(fileStream);
                    }

                    string fileRelativeUrl = "/images/" + fileName;
                    ViewData["ImageUrl"] = fileRelativeUrl;
                }
            }

            // Return the view with the updated ViewData
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
