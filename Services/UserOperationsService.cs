using JATS.Data;
using JATS.Models;
using JATS.Models.ViewModel;
using JATS.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;


namespace JATS.Services
{
    public class UserOperationsService : IUserOperationsService
    {
        private readonly UserManager<JTUser> _userManager;
        private readonly IUserStore<JTUser> _userStore;
        private readonly IUserEmailStore<JTUser> _emailStore;
        private readonly IRolesService _roleService;
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        public UserOperationsService(UserManager<JTUser> userManager,
            IUserStore<JTUser> userStore, IRolesService rolesService,
            ApplicationDbContext context, IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _roleService = rolesService;
            _context = context;
            _emailSender = emailSender;
        }
        public async Task<bool> AddCompanyUserWithRolesAsync(AddCompanyUserViewModel companyUser, int companyId)
        {

            var user = CreateUser();
            user.FirstName = companyUser.FirstName;
            user.LastName = companyUser.LastName;
            user.CompanyId = companyId;
            user.Email = companyUser.Email;

            IdentityResult result;

            try
            {
                await _userStore.SetUserNameAsync(user, companyUser.Email, CancellationToken.None);
                //await _emailStore.SetEmailAsync(user, companyUser.Email, CancellationToken.None);
                result = await _userManager.CreateAsync(user, companyUser.Password);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return false;
            }

            if (result.Succeeded)
            {
                try
                {
                    foreach (var role in companyUser.SelectedRoles)
                    {
                        await _roleService.AddUserToRole(user, role);
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }


        }

        private JTUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<JTUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(JTUser)}'. " +
                    $"Ensure that '{nameof(JTUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }




    }
}
