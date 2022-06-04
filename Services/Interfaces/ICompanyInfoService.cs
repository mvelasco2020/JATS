using JATS.Models;

namespace JATS.Services.Interfaces
{
    public interface ICompanyInfoService
    {
        public Task<Company> GetCompanyByIdAsync(int? companyId);

        public Task<List<JTUser>> GetAllMembersAsync(int companyId);

        public Task<List<Project>> GetAllProjectsAsync(int companyId);

        public Task<List<Ticket>> GetAllTicketsAsync(int companyId);


    }
}
