using JATS.Models;
using Microsoft.AspNetCore.Identity;

namespace JATS.Services.Interfaces
{
    public interface IRolesService
    {
        public Task<bool> IsUserInRoleAsync(JTUser user, string roleName);

        public Task<List<IdentityRole>> GetAllRolesAsync();
        public Task<IEnumerable<string>> GetUserRolesAsync(JTUser user);

        public Task<bool> AddUserToRole(JTUser user, string roleName);


        public Task<bool> RemoveUserFromRoleAsync(JTUser user, string roleName);

        public Task<bool> RemoveUserFromRolesAsync(JTUser user, IEnumerable<string> roles);

        public Task<List<JTUser>> GetUsersInRoleAsync(string roleName, int companyId);

        public Task<List<JTUser>> GetUsersNotInRoleAsync(string roleName, int companyId);

        public Task<string> GetRoleNameByIdAsync(string roleId);

    }
}
