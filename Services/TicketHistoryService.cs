using JATS.Data;
using JATS.Models;
using JATS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JATS.Services
{
    public class TicketHistoryService : ITicketHistoryService
    {
        private readonly ApplicationDbContext _context;
        public TicketHistoryService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddHistoryAsync(Ticket oldTicket, Ticket newTicket, string userId)
        {
            if (oldTicket is null && newTicket is not null)
            {
                TicketHistory history = new()
                {
                    TicketId = newTicket.Id,
                    UserId = userId,
                    Created = DateTimeOffset.Now,
                    Property = "",
                    Description = "New ticket created.",
                    OldValue = "",
                    NewValue = ""
                };

                try
                {
                    await _context.TicketHistories.AddAsync(history);
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            {


                if (oldTicket.Title != newTicket.Title)
                {
                    TicketHistory history = new();
                    history.TicketId = newTicket.Id;
                    history.UserId = userId;
                    history.Created = DateTimeOffset.Now;
                    history.Property = "title";
                    history.Description = $"New ticket title: {newTicket.Title}.";
                    history.OldValue = oldTicket.Title;
                    history.NewValue = newTicket.Title;
                    await _context.TicketHistories.AddAsync(history);

                }
                if (oldTicket.Description != newTicket.Description)
                {
                    TicketHistory history = new();
                    history.TicketId = newTicket.Id;
                    history.UserId = userId;
                    history.Created = DateTimeOffset.Now;
                    history.Property = "Description";
                    history.Description = $"New ticket title: {newTicket.Title}.";
                    history.OldValue = oldTicket.Title;
                    history.NewValue = newTicket.Title;
                    await _context.TicketHistories.AddAsync(history);

                }
                if (oldTicket.TicketPriorityId != newTicket.TicketPriorityId)
                {
                    TicketHistory history = new();
                    history.TicketId = newTicket.Id;
                    history.UserId = userId;
                    history.Created = DateTimeOffset.Now;
                    history.Property = "TicketPriority";
                    history.Description = $"New ticket priority: {newTicket.TicketPriority.Name}.";
                    history.OldValue = oldTicket.TicketPriority.Name;
                    history.NewValue = newTicket.TicketPriority.Name;
                    await _context.TicketHistories.AddAsync(history);

                }

                if (oldTicket.TicketStatusId != newTicket.TicketStatusId)
                {
                    TicketHistory history = new();
                    history.TicketId = newTicket.Id;
                    history.UserId = userId;
                    history.Created = DateTimeOffset.Now;
                    history.Property = "TicketStatus";
                    history.Description = $"New ticket status: {newTicket.TicketStatus.Name}.";
                    history.OldValue = oldTicket.TicketStatus.Name;
                    history.NewValue = newTicket.TicketStatus.Name;
                    await _context.TicketHistories.AddAsync(history);

                }

                if (oldTicket.TicketTypeId != newTicket.TicketTypeId)
                {
                    TicketHistory history = new();
                    history.TicketId = newTicket.Id;
                    history.UserId = userId;
                    history.Created = DateTimeOffset.Now;
                    history.Property = "TicketType";
                    history.Description = $"New ticket type: {newTicket.TicketType.Name}.";
                    history.OldValue = oldTicket.TicketType.Name;
                    history.NewValue = newTicket.TicketType.Name;
                    await _context.TicketHistories.AddAsync(history);

                }

                if (oldTicket.TechnicianUserId != newTicket.TechnicianUserId)
                {
                    TicketHistory history = new();
                    history.TicketId = newTicket.Id;
                    history.UserId = userId;
                    history.Created = DateTimeOffset.Now;
                    history.Property = "Technician";
                    history.Description = $"New ticket technician: {newTicket.TechnicianUser?.FullName}.";
                    history.OldValue = oldTicket.TechnicianUser?.FullName ?? "Not Assigned";
                    history.NewValue = newTicket.TechnicianUser?.FullName;
                    await _context.TicketHistories.AddAsync(history);
                }

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {

                    throw;
                }



            }
        }

        public async Task AddHistoryAsync(int ticketId, string model, string userId)
        {
            try
            {
                Ticket ticket = await _context.Tickets.FindAsync(ticketId);
                string description = model.ToLower().Replace("Ticket", "");
                description = $"New {description} add to ticket: {ticket.Title}";

                TicketHistory history = new()
                {
                    TicketId = ticket.Id,
                    Property = model,
                    UserId = userId,
                    OldValue = "",
                    NewValue = "",
                    Created = DateTimeOffset.Now,
                    Description = description
                };

                await _context.TicketHistories.AddAsync(history);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<TicketHistory>> GetCompanyTicketsHistoriesAsync(int companyId)
        {
            try
            {
                List<Project> projects = (await _context
                    .Companies
                    .Include(c => c.Projects)
                    .ThenInclude(p => p.Tickets)
                    .ThenInclude(t => t.History)
                    .ThenInclude(h => h.User)
                    .FirstOrDefaultAsync(c => c.Id == companyId))
                    .Projects
                    .ToList();

                List<Ticket> tickets = projects
                    .SelectMany(p => p.Tickets)
                    .ToList();

                return tickets
                    .SelectMany(t => t.History)
                    .ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<TicketHistory>> GetProjectTicketsHistoriesAsync(int projectId, int companyId)
        {
            try
            {
                Project project = await _context
                    .Projects.Where(p => p.CompanyId == companyId)
                    .Include(p => p.Tickets)
                    .ThenInclude(t => t.History)
                    .ThenInclude(th => th.User)
                    .FirstOrDefaultAsync(p => p.Id == projectId);

                return project.Tickets.SelectMany(t => t.History).ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
