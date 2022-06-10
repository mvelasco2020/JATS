using JATS.Data;
using JATS.Models;
using JATS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JATS.Services
{
    public class LookupService : ILookupService
    {
        private readonly ApplicationDbContext _context;
        public LookupService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<ProjectPriority>> GetProjectPrioritiesAsync()
        {
            return await _context.ProjectPriorities.ToListAsync();
        }

        public Task<List<TicketPriority>> GetTicketPriorituesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<TicketStatus>> GetTicketStatusesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<TicketType>> GetTicketTypesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
