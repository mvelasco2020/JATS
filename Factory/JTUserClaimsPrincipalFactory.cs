using JATS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace JATS.Factory
{
    public class JTUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<JTUser, IdentityRole>
    {

        public JTUserClaimsPrincipalFactory(UserManager<JTUser> userManager,
                                            RoleManager<IdentityRole> roleManager,
                                            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {

        }


        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(JTUser user)
        {
            ClaimsIdentity identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("CompanyId", user.CompanyId.ToString()));
            return identity;
        }
    }
}
