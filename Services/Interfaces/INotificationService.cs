using JATS.Models;

namespace JATS.Services.Interfaces
{
    public interface INotificationService
    {

        public Task AddNotificationAsync(Notification notification);

        public Task<List<Notification>> GetRecievedNotificationsAsync(string userId);

        public Task<List<Notification>> GetSentNotificationsAsync(string userId);

        public Task SendEmailNotificationsByRoleAsync(Notification notification, int companyId, string role);

        public Task SendMembersEmailNotificationsAsync(Notification notification, List<JTUser> jTUsers);

        public Task<bool> SendEmailNotificationAsync(Notification notification, string emailSubject);
    }
}
