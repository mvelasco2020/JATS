using JATS.Data;
using JATS.Models;
using JATS.Models.Enums;
using JATS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace JATS.Services
{
    public class TicketService : ITicketService
    {

        private readonly ApplicationDbContext _context;
        private readonly IRolesService _roleService;
        private readonly IProjectService _projectService;

        public TicketService(ApplicationDbContext context,
            IRolesService roleService,
            IProjectService projectService)
        {
            _context = context;
            _roleService = roleService;
            _projectService = projectService;
        }
        public async Task AddNewTicketAsync(Ticket ticket)
        {
            try
            {
                await _context.AddAsync(ticket);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task ArchiveTicketAsync(Ticket ticket)
        {
            try
            {
                ticket.Archived = true;
                _context.Update(ticket);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                throw;

            }
        }
        public async Task AssignTicketAsync(int ticketId, string userId)
        {
            Ticket ticket = await GetTicketByIdAsync(ticketId);
            try
            {
                if (ticket is not null)
                {
                    try
                    {
                        ticket.TechnicianUserId = userId;
                        ticket.TicketStatusId = (await LookupTicketStatusIdAsync("InProgress")).Value;
                        _context.Update(ticket);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByCompanyAsync(int companyId)
        {
            try
            {
                List<Ticket> tickets = await _context
                      .Projects
                      .Where(p => p.CompanyId == companyId)
                      .SelectMany(t => t.Tickets)
                      .Include(t => t.Attachments)
                      .Include(t => t.Comments)
                      .Include(t => t.TechnicianUser)
                      .Include(t => t.History)
                      .Include(t => t.OwnerUser)
                      .Include(t => t.TicketPriority)
                      .Include(t => t.TicketStatus)
                      .Include(t => t.TicketType)
                      .Include(t => t.Project)
                      .ToListAsync();
                return tickets;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByPriorityAsync(int companyId, string priorityName)
        {
            int priorityId = (await LookupTicketPriorityIdAsync(priorityName)).Value;

            try
            {
                return await _context
                     .Projects
                     .Where(p => p.CompanyId == companyId)
                     .SelectMany(t => t.Tickets)
                      .Include(t => t.Attachments)
                      .Include(t => t.Comments)
                      .Include(t => t.TechnicianUser)
                      .Include(t => t.History)
                      .Include(t => t.OwnerUser)
                      .Include(t => t.TicketPriority)
                      .Include(t => t.TicketStatus)
                      .Include(t => t.TicketType)
                      .Include(t => t.Project)
                     .Where(p => p.TicketPriority.Id == priorityId)
                     .ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByStatusAsync(int companyId, string statusName)
        {
            int statusId = (await LookupTicketStatusIdAsync(statusName)).Value;
            try
            {
                return await _context
                    .Projects
                    .Where(p => p.CompanyId == companyId)
                    .SelectMany(p => p.Tickets)
                      .Include(t => t.Attachments)
                      .Include(t => t.Comments)
                      .Include(t => t.TechnicianUser)
                      .Include(t => t.History)
                      .Include(t => t.OwnerUser)
                      .Include(t => t.TicketPriority)
                      .Include(t => t.TicketStatus)
                      .Include(t => t.TicketType)
                      .Include(t => t.Project)
                     .Where(p => p.TicketStatusId == statusId)
                    .ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByTypeAsync(int companyId, string typeName)
        {
            int ticketTypeId = (await LookupTicketTypeIdAsync(typeName)).Value;

            try
            {
                return await _context
                      .Projects
                      .Where(p => p.CompanyId == companyId)
                      .SelectMany(p => p.Tickets)
                      .Include(t => t.Attachments)
                      .Include(t => t.Comments)
                      .Include(t => t.TechnicianUser)
                      .Include(t => t.History)
                      .Include(t => t.OwnerUser)
                      .Include(t => t.TicketPriority)
                      .Include(t => t.TicketStatus)
                      .Include(t => t.TicketType)
                      .Include(t => t.Project)
                      .Where(t => t.TicketTypeId == ticketTypeId)
                      .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<List<Ticket>> GetArchivedTicketsAsync(int companyId)
        {
            try
            {
                return (await GetAllTicketsByCompanyAsync(companyId))
                    .Where(t => t.Archived == true)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<List<Ticket>> GetProjectTicketsByPriorityAsync(string priorityName, int companyId, int projectId)
        {
            try
            {
                return (await GetAllTicketsByPriorityAsync(companyId, priorityName))
                      .Where(t => t.ProjectId == projectId)
                      .ToList();

            }
            catch (Exception ex)
            {
                Console.Write(ex);

                throw;
            }
        }

        public async Task<List<Ticket>> GetProjectTicketsByRoleAsync(string role,
                                                                string userId,
                                                                int projectId,
                                                                int companyId)
        {
            List<Ticket> tickets = new();
            try
            {
                tickets = (await GetTicketsByRoleAsync(role, userId, companyId)).
                    Where(t => t.ProjectId == projectId)
                    .ToList();

                return tickets;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                throw;
            }
        }

        public async Task<List<Ticket>> GetProjectTicketsByStatusAsync(string statusName, int companyId, int projectId)
        {

            try
            {
                return (await GetAllTicketsByStatusAsync(companyId, statusName))
                    .Where(t => t.ProjectId == projectId)
                    .ToList();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<List<Ticket>> GetProjectTicketsByTypeAsync(string typeName, int companyId, int projectId)
        {

            try
            {
                return (await GetAllTicketsByTypeAsync(companyId, typeName))
                      .Where(t => t.ProjectId == projectId)
                      .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<Ticket> GetTicketByIdAsync(int ticketId)
        {
            return await _context
                .Tickets
                .Include(t => t.TechnicianUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.Project)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .Include(t => t.Comments)
                .ThenInclude(c => c.User)
                .Include(t => t.History)
                .Include(t => t.Attachments)
                .FirstOrDefaultAsync(t => t.Id == ticketId);
        }

        public async Task<JTUser> GetTicketTechnicianAsync(int ticketId, int companyId)
        {
            try
            {
                Ticket ticket = (await GetAllTicketsByCompanyAsync(companyId))
                   .FirstOrDefault(t => t.Id == ticketId);

                if (ticket is not null && ticket?.TechnicianUser is not null)
                {
                    return ticket.TechnicianUser;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                throw;
            }
        }

        public async Task<List<Ticket>> GetTicketsByRoleAsync(string role, string userId, int companyId)
        {
            List<Ticket> tickets = new();
            try
            {
                if (role == Roles.Admin.ToString())
                {
                    tickets = await GetAllTicketsByCompanyAsync(companyId);
                }
                else if (role == Roles.Technician.ToString())
                {
                    tickets = (await GetAllTicketsByCompanyAsync(companyId))
                        .Where(t => t.TechnicianUserId == userId)
                        .ToList();
                }
                else if (role == Roles.ProjectManager.ToString())
                {
                    tickets = await GetTicketsByUserIdAsync(userId, companyId);
                }
                else if (role == Roles.Submitter.ToString())
                {
                    tickets = (await GetAllTicketsByCompanyAsync(companyId))
                        .Where(t => t.OwnerUserId == userId)
                        .ToList();
                }
                return tickets;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<List<Ticket>> GetTicketsByUserIdAsync(string userId, int companyId)
        {
            JTUser user = await _context
                .Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            List<Ticket> tickets = new();
            try
            {
                if (await _roleService.IsUserInRoleAsync(user, Roles.Admin.ToString()))
                {
                    tickets = (await _projectService.GetAllProjectsByCompany(companyId))
                        .SelectMany(pt => pt.Tickets)
                        .ToList();
                }
                else if (await _roleService.IsUserInRoleAsync(user, Roles.Technician.ToString()))
                {
                    tickets = (await _projectService.GetAllProjectsByCompany(companyId))
                        .SelectMany(pt => pt.Tickets)
                        .Where(t => t.TechnicianUserId == userId)
                        .ToList();
                }
                else if (await _roleService.IsUserInRoleAsync(user, Roles.ProjectManager.ToString()))
                {
                    tickets = (await _projectService.GetUserProjectsAsync(userId))
                        .SelectMany(pt => pt.Tickets)
                        .ToList();
                }
                else if (await _roleService.IsUserInRoleAsync(user, Roles.Submitter.ToString()))
                {
                    tickets = (await _projectService.GetAllProjectsByCompany(companyId))
                        .SelectMany(pt => pt.Tickets)
                        .Where(t => t.OwnerUserId == userId)
                        .ToList();
                };
                return tickets;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<int?> LookupTicketPriorityIdAsync(string priorityName)
        {
            try
            {
                return (await _context
                    .TicketPriorities
                    .FirstOrDefaultAsync(p => p.Name == priorityName))?.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<int?> LookupTicketStatusIdAsync(string statusName)
        {
            try
            {
                return (await _context
                              .TicketStatuses
                              .FirstOrDefaultAsync(ts => ts.Name == statusName))?.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

        }

        public async Task<int?> LookupTicketTypeIdAsync(string ticketName)
        {
            try
            {
                return (await _context
                    .TicketTypes
                    .FirstOrDefaultAsync(tn => tn.Name == ticketName)).Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task UpdateTicketAsync(Ticket ticket)
        {
            try
            {
                _context.Update(ticket);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task AddTicketCommentAsync(TicketComment ticketComment)
        {
            try
            {
                await _context.AddAsync(ticketComment);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task AddTicketAttachmentAsync(TicketAttachment ticketAttachment)
        {
            try
            {
                await _context.AddAsync(ticketAttachment);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<TicketAttachment> GetTicketAttachmentByIdAsync(int ticketAttachmentId)
        {
            try
            {
                TicketAttachment ticketAttachment = await _context.TicketAttachments
                                                                  .Include(t => t.User)
                                                                  .FirstOrDefaultAsync(t => t.Id == ticketAttachmentId);
                return ticketAttachment;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Ticket>> GetUnassignedTicketsAsync(int companyId)
        {
            try
            {
                return (await GetAllTicketsByCompanyAsync(companyId))
                            .Where(t => string.IsNullOrEmpty(t.TechnicianUserId))
                            .ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Ticket> GetTicketAsNoTrackingAsync(int ticketId)
        {
            try
            {
                return await _context
    .Tickets
    .Include(t => t.TechnicianUser)
    .Include(t => t.Project)
    .Include(t => t.TicketPriority)
    .Include(t => t.TicketStatus)
    .Include(t => t.TicketType)
    .Include(t => t.Comments)
    .ThenInclude(c => c.User)
    .AsNoTracking()
    .FirstOrDefaultAsync(t => t.Id == ticketId);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
