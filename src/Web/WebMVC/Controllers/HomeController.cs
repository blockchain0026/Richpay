using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Models.Permissions;

namespace WebMVC.Controllers
{
    public class HomeController : Controller
    {
        [Authorize(Policy = Permissions.Additional.Reviewed)]
        [Authorize(Policy = Permissions.Dashboards.View)]
        public IActionResult Index()
        {
            //return RedirectToAction("Index", "Order");
            Console.WriteLine("Go To Home Index View");
            Console.WriteLine(HttpContext.Request.Cookies.FirstOrDefault().ToString());
            return View();
        }    
        
        public IActionResult ApiDoc()
        {
            //return RedirectToAction("Index", "Order");
            return View();
        }

        public IActionResult ApiTest()
        {
            return View();
        }

    }
}