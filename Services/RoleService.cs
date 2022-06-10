using JATS.Data;
using JATS.Models;
using JATS.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JATS.Services
{
    public class RoleService : IRolesService
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<JTUser> _userManager;
        public RoleService(ApplicationDbContext context,
                            RoleManager<IdentityRole> roleManager,
                            UserManager<JTUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task<bool> AddUserToRole(JTUser user, string roleName)
        {
            bool result = (await _userManager.AddToRoleAsync(user, roleName)).Succeeded;

            return result;

        }

        public async Task<List<IdentityRole>> GetAllRolesAsync()
        {
            try
            {
                return await _context.Roles.ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<string> GetRoleNameByIdAsync(string roleId)
        {
            IdentityRole role = _context.Roles.Find(roleId);
            string result = await _roleManager.GetRoleNameAsync(role);
            return result;
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(JTUser user)
        {
            IEnumerable<string> result = await _userManager.GetRolesAsync(user);
            return result;
        }

        public async Task<List<JTUser>> GetUsersInRoleAsync(string roleName, int companyId)
        {
            List<JTUser> users = (await _userManager.GetUsersInRoleAsync(roleName)).ToList();
            List<JTUser> usersInCompany = users.Where(x => x.CompanyId == companyId).ToList();
            return usersInCompany;
        }

        public async Task<List<JTUser>> GetUsersNotInRoleAsync(string roleName, int companyId)
        {
            List<JTUser> usersInCompany = _context.Users
                .Where(u => u.CompanyId == companyId).ToList();
            List<JTUser> usersInRole = (await _userManager.GetUsersInRoleAsync(roleName)).ToList();
            List<JTUser> usersNotInRole = usersInCompany.Where(x => !usersInRole.Contains(x)).ToList();

            return usersNotInRole;
        }

        public async Task<bool> IsUserInRoleAsync(JTUser user, string roleName)
        {

            bool result = await _userManager.IsInRoleAsync(user, roleName);
            return result;
        }

        public async Task<bool> RemoveUserFromRoleAsync(JTUser user, string roleName)
        {
            bool result = (await _userManager.RemoveFromRoleAsync(user, roleName)).Succeeded;
            return result;
        }

        public async Task<bool> RemoveUserFromRolesAsync(JTUser user, IEnumerable<string> roles)
        {
            bool result = (await _userManager.RemoveFromRolesAsync(user, roles)).Succeeded;
            return result;

        }
    }
}
