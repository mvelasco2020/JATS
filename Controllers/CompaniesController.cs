using JATS.Data;
using JATS.Extensions;
using JATS.Models;
using JATS.Models.ViewModel;
using JATS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace JATS.Controllers
{
    [Authorize]
    public class CompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICompanyInfoService _companyService;
        private readonly IRolesService _roleService;

        public CompaniesController(ApplicationDbContext context,
            ICompanyInfoService companyService,
            IRolesService roleService)
        {
            _context = context;
            _companyService = companyService;
            _roleService = roleService;
        }

        // GET: Companies
        public async Task<IActionResult> Index()
        {

            Company company = await _companyService
                .GetCompanyByIdAsync(User.Identity.GetCompanyId().Value);

            return View(company);
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Companies == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] Company company)
        {
            if (ModelState.IsValid)
            {
                _context.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Companies == null)
            {
                return NotFound();
            }

            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Company company)
        {
            if (id != company.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        [HttpGet]
        public async Task<IActionResult> AddCompanyUser()
        {

            ManageUserRolesViewModel model = new();
            model.Roles = new MultiSelectList(await _roleService.GetAllRolesAsync(), "Name", "Name");
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddCompanyUser(ManageUserRolesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var companyId = User.Identity.GetCompanyId().Value;
                model.JTUser.CompanyId = companyId;
                try
                {
                    _context.Users.Add(model.JTUser);
                    await _context.SaveChangesAsync();
                    foreach (var role in model.SelectedRoles)
                    {
                        await _roleService.AddUserToRole(model.JTUser, role);
                    }
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                    return View("Error");
                }
            }

            return View(model);
        }


        private bool CompanyExists(int id)
        {
            return (_context.Companies?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
