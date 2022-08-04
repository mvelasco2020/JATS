using JATS.Models.ViewModel;

namespace JATS.Services.Interfaces
{
    public interface IUserOperationsService
    {
        public Task<bool> AddCompanyUserWithRolesAsync(AddCompanyUserViewModel companyUser, int companyId);
    }
}
