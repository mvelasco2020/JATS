using JATS.Extensions;
using JATS.Models;
using JATS.Models.ViewModel;
using JATS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace JATS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICompanyInfoService _companyInfoService;
        private readonly IProjectService _projectService;


        public HomeController(ILogger<HomeController> logger,
                                ICompanyInfoService companyInfoService,
                                IProjectService projectService)
        {
            _logger = logger;
            _companyInfoService = companyInfoService;
            _projectService = projectService;
        }

        public IActionResult Index()
        {
            return View();
        }


        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            DashboardViewModel model = new DashboardViewModel();
            int companyId = User.Identity.GetCompanyId().Value;
            model.Company = await _companyInfoService.GetCompanyByIdAsync(companyId);

            model.Projects = (await _projectService
                .GetAllProjectsByCompany(companyId))
                .Where(p => p.Archived == false)
                .ToList();

            model.Tickets = model.Projects
                .SelectMany(p => p.Tickets)
                .Where(p => p.Archived == false)
                .ToList();

            model.Users = model.Company.Members.ToList();

            return View(model);
        }


        [HttpPost]
        public async Task<JsonResult> GglProjectTickets()
        {
            int companyId = User.Identity.GetCompanyId().Value;

            List<Project> projects = await _projectService.GetAllProjectsByCompany(companyId);

            List<object> chartData = new();
            chartData.Add(new object[] { "ProjectName", "TicketCount" });

            foreach (Project prj in projects)
            {
                chartData.Add(new object[] { prj.Name, prj.Tickets.Count() });
            }

            return Json(chartData);
        }

        [HttpPost]
        public async Task<JsonResult> GglProjectPriority()
        {
            int companyId = User.Identity.GetCompanyId().Value;

            List<Project> projects = await _projectService.GetAllProjectsByCompany(companyId);

            List<object> chartData = new();
            chartData.Add(new object[] { "Priority", "Count" });


            foreach (string priority in Enum.GetNames(typeof(ProjectPriority)))
            {
                int priorityCount = (await _projectService.GetAllProjectsByPriority(companyId, priority)).Count();
                chartData.Add(new object[] { priority, priorityCount });
            }

            return Json(chartData);
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