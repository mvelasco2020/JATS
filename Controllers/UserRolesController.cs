using JATS.Extensions;
using JATS.Models;
using JATS.Models.ViewModel;
using JATS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace JATS.Controllers
{
    [Authorize]
    public class UserRolesController : Controller
    {
        private readonly IRolesService _roleService;
        private readonly ICompanyInfoService _companyInfoService;

        public UserRolesController(IRolesService roleService,
                                   ICompanyInfoService companyInfoService)
        {
            _roleService = roleService;
            _companyInfoService = companyInfoService;
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles()
        {
            List<ManageUserRolesViewModel> model = new();

            int companyId = User.Identity.GetCompanyId().Value;

            List<JTUser> users = await _companyInfoService.GetAllMembersAsync(companyId);

            foreach (JTUser user in users)
            {
                ManageUserRolesViewModel viewModel = new();

                viewModel.JTUser = user;
                IEnumerable<string> selected = await _roleService
                    .GetUserRolesAsync(user);

                viewModel.Roles = new MultiSelectList(await _roleService
                    .GetAllRolesAsync(), "Name", "Name", selected);

                model.Add(viewModel);

            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel model)
        {
            int companyId = User.Identity.GetCompanyId().Value;
            JTUser user = (await _companyInfoService
                .GetAllMembersAsync(companyId))
                .FirstOrDefault(u => u.Id == model.JTUser.Id);

            IEnumerable<string> roles = await _roleService.GetUserRolesAsync(user);

            string userRole = model.SelectedRoles.FirstOrDefault();

            if (!string.IsNullOrEmpty(userRole))
            {
                if (await _roleService.RemoveUserFromRolesAsync(user, roles))
                {
                    await _roleService.AddUserToRole(user, userRole);
                }
            }

            return RedirectToAction(nameof(ManageUserRoles));
        }


    }
}
