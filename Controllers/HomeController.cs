using dipwebapp.Models;
using dipwebapp.Models.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;

namespace dipwebapp.Controllers
{    
    public class HomeController : Controller
    {
        SiteRepository _siteRepository = new SiteRepository();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            IndexViewModel vm = new IndexViewModel();
            int? userID = null;
            if (HttpContext.Session.GetInt32("_UserID") != null)
            {
                userID = HttpContext.Session.GetInt32("_UserID");
                vm.CurrentUser = _siteRepository.GetUser((int)userID);
            }
            return View(vm);
        }
        public IActionResult UserMessage(string msg)
        {
            return View(msg);
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
