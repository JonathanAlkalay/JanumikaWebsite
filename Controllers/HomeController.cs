using JanumikaPro.Data;
using JanumikaPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace JanumikaPro.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly JanumikaProContext _context;
        public HomeController(ILogger<HomeController> logger,JanumikaProContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Index()
        {
            //List<Category> categories = _context.Category.Include(a => a.Image).ToList();
            ViewData["categories"] = _context.Category.Include(a => a.Image);
            return View();
        }
        //[Authorize(Roles = "Admin")]
        public IActionResult Privacy()
        {
            /*
             if (HttpContext.Session.GetString("username") == null)
            {
                return RedirectToAction("Login", "Users");
            }
             */
            return View();
        }

        public ActionResult CartSize()
        {
            String userName = HttpContext.User.Identity.Name;
            User user = _context.User.FirstOrDefault(x => x.UserName.Equals(userName));
            Cart cart = _context.Cart.Include(db => db.Items).FirstOrDefault(x => x.UserId == user.Id);

            return PartialView("CartSize", cart);

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult VisitUs()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }
    }
}

