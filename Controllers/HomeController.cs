using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RecruitAPP.Models;

namespace RecruitAPP.Controllers
{
    /// <summary>
    /// Home controller for landing pages and redirects
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// GET: /
        /// Redirects authenticated users to their dashboard or shows the public home page.
        /// </summary>
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Dashboard", "Admin");
                }

                if (User.IsInRole("Recruteur"))
                {
                    return RedirectToAction("Dashboard", "Recruteur");
                }

                return RedirectToAction("Index", "Offres");
            }

            return View();
        }

        /// <summary>
        /// GET: /Home/Privacy
        /// Shows privacy information.
        /// </summary>
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        /// <summary>
        /// GET: /Home/Error
        /// Error page.
        /// </summary>
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
