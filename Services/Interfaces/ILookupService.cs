using JATS.Models;

namespace JATS.Services.Interfaces
{
    public interface ILookupService
    {
        public Task<List<TicketPriority>> GetTicketPriorituesAsync();
        public Task<List<TicketStatus>> GetTicketStatusesAsync();
        public Task<List<TicketType>> GetTicketTypesAsync();

        public Task<List<ProjectPriority>> GetProjectPrioritiesAsync();
    }
}
