using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InitialSetupMVC.Services;
using InitialSetupMVC.Models;
using System.Diagnostics;

namespace InitialSetupMVC.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly DashboardService _dashboardService;

        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public IActionResult Index()
        {
            var data = _dashboardService.GetDashboardData();
            return View(data);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
