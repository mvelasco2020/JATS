namespace JATS.Models.ViewModel
{
    public class DashboardViewModel
    {
        public Company Company { get; set; }

        public List<Project> Projects { get; set; }

        public List<Ticket> Tickets { get; set; }

        public List<JTUser> Users { get; set; }
    }
}
