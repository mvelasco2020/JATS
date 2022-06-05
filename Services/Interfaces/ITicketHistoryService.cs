﻿using JATS.Models;

namespace JATS.Services.Interfaces
{
    public interface ITicketHistoryService
    {

        Task AddHistoryAsync(Ticket oldTicket, Ticket newTicket, string userId);
        Task<List<TicketHistory>> GetProjectTicketsHistoriesAsync(int projectId, int companyId);

        Task<List<TicketHistory>> GetCompanyTicketsHistoriesAsync(int companyId);
    }
}
