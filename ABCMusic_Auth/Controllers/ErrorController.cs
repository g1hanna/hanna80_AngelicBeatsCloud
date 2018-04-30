using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ABCMusic_Auth
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("AccessDenied");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}