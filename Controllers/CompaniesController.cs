﻿using JATS.Data;
using JATS.Extensions;
using JATS.Models;
using JATS.Models.ViewModel;
using JATS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using System.Text.RegularExpressions;

namespace JATS.Controllers
{
    [Authorize]
    public class CompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICompanyInfoService _companyService;
        private readonly IRolesService _roleService;
        private readonly UserManager<JTUser> _userManager;
        private readonly IUserOperationsService _userOpsService;

        public CompaniesController(ApplicationDbContext context,
            ICompanyInfoService companyService,
            IRolesService roleService,
            UserManager<JTUser> userManager,
            IUserOperationsService userOpsService)
        {
            _context = context;
            _companyService = companyService;
            _roleService = roleService;
            _userManager = userManager;
            _userOpsService = userOpsService;
        }

        // GET: Companies
        public async Task<IActionResult> Index()
        {

            Company company = await _companyService
                .GetCompanyByIdAsync(User.Identity.GetCompanyId().Value);

            return View(company);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> EditCompanyDetails()
        {
            Company company = await _companyService
                .GetCompanyByIdAsync(User.Identity.GetCompanyId().Value);

            return View(company);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> EditCompanyDetails(Company company)
        {
            Company userCompany = await _companyService
                .GetCompanyByIdAsync(User.Identity.GetCompanyId().Value);
            if (company.Id == userCompany.Id)
            {
                userCompany.Name = company.Name;
                userCompany.Description = company.Description;
                userCompany.SocialMediaFacebook = company.SocialMediaFacebook;
                userCompany.SocialMediaInstagram = company.SocialMediaInstagram;
                userCompany.SocialMediaTwitter = company.SocialMediaTwitter;
                userCompany.PhoneNumber = company.PhoneNumber;
                userCompany.Email = company.Email;
                try
                {
                    _context.Update(userCompany);
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> AddCompanyUser()
        {

            AddCompanyUserViewModel model = new();
            model.Roles = new MultiSelectList(await _roleService.GetAllRolesAsync(), "Name", "Name");
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddCompanyUser(AddCompanyUserViewModel model)
        {
            model.Roles = new MultiSelectList(await _roleService.GetAllRolesAsync(), "Name", "Name", model.SelectedRoles);

            if (ModelState.IsValid)
            {
                if (!IsValidEmail(model.Email))
                {
                    ModelState.AddModelError("UserError", $" '{model.Email}' is not a valid email address");
                }

                if ((await _userManager.FindByEmailAsync(model.Email)) != null)
                {
                    ModelState.AddModelError("UserError", $"Email '{model.Email}' is already taken.");
                }

                var companyId = User.Identity.GetCompanyId().Value;
                bool result = await _userOpsService.AddCompanyUserWithRolesAsync(model, companyId);

                if (result == true)
                {
                    model.Roles = new MultiSelectList(await _roleService.GetAllRolesAsync(), "Name", "Name");
                    ViewData["SucessMessage"] = "Sucessfully created a new user account.";
                }
                else
                {
                    ViewData["ErrorMessage"] = "Something went wrong while creating a new account, Please check if all fields have a correct input.";

                }
            }

            return View(model);
        }

        private bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; // suggested by @TK-421
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                if (string.IsNullOrWhiteSpace(email))
                    return false;

                try
                {
                    // Normalize the domain
                    email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                          RegexOptions.None, TimeSpan.FromMilliseconds(200));

                    // Examines the domain part of the email and normalizes it.
                    string DomainMapper(Match match)
                    {
                        // Use IdnMapping class to convert Unicode domain names.
                        var idn = new IdnMapping();

                        // Pull out and process domain name (throws ArgumentException on invalid)
                        string domainName = idn.GetAscii(match.Groups[2].Value);

                        return match.Groups[1].Value + domainName;
                    }
                }
                catch (RegexMatchTimeoutException e)
                {
                    return false;
                }
                catch (ArgumentException e)
                {
                    return false;
                }

                try
                {
                    return Regex.IsMatch(email,
                        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                        RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
                }
                catch (RegexMatchTimeoutException)
                {
                    return false;
                }
            }
        }

    }
}
