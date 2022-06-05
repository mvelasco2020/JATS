using JATS.Data;
using JATS.Models;
using JATS.Services.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace JATS.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IRolesService _rolesService;
        public NotificationService(ApplicationDbContext context,
            IEmailSender emailSender,
            IRolesService rolesService
          )
        {
            _context = context;
            _emailSender = emailSender;
            _rolesService = rolesService;
        }
        public async Task AddNotificationAsync(Notification notification)
        {
            try
            {
                await _context.AddAsync(notification);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Notification>> GetRecievedNotificationsAsync(string userId)
        {
            try
            {
                return await _context
                    .Notifications
                    .Include(n => n.Recipient)
                    .Include(n => n.Sender)
                    .Include(n => n.Ticket)
                    .ThenInclude(t => t.Project)
                    .Where(u => u.RecipientId == userId)
                    .ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Notification>> GetSentNotificationsAsync(string userId)
        {
            try
            {
                return await _context
                             .Notifications
                             .Include(n => n.Recipient)
                             .Include(n => n.Sender)
                             .Include(n => n.Ticket)
                             .ThenInclude(t => t.Project)
                             .Where(u => u.SenderId == userId)
                             .ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> SendEmailNotificationAsync(Notification notification, string emailSubject)
        {
            JTUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == notification.RecipientId);

            if (user is not null)
            {

                try
                {
                    await _emailSender.SendEmailAsync(user.Email, emailSubject, notification.Message);
                    return true;

                }
                catch (Exception)
                {
                    return false;
                    throw;
                }
            }

            return false;

        }

        public async Task SendEmailNotificationsByRoleAsync(Notification notification, int companyId, string role)
        {
            try
            {
                List<JTUser> members = await _rolesService.GetUsersInRoleAsync(role, companyId);

                foreach (var member in members)
                {
                    try
                    {
                        await SendEmailNotificationAsync(notification, notification.Title);

                    }
                    catch (Exception)
                    {
                        throw;

                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task SendMembersEmailNotificationsAsync(Notification notification, List<JTUser> jTUsers)
        {


            foreach (var member in jTUsers)
            {
                try
                {
                    await SendEmailNotificationAsync(notification, notification.Title);

                }
                catch (Exception)
                {
                    throw;

                }
            }

        }

    }
}
