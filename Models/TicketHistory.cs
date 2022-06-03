using System.ComponentModel;

namespace JATS.Models
{
    public class TicketHistory
    {

        public int Id { get; set; }

        public int TicketId { get; set; }

        [DisplayName("Updated Item")]
        public string Property { get; set; }

        [DisplayName("Previous")]
        public string OldValue { get; set; }

        [DisplayName("Current")]
        public string NewValue { get; set; }

        [DisplayName("Date Modified")]
        public DateTimeOffset Created { get; set; }

        [DisplayName("Description of change")]
        public string Description { get; set; }

        //Navigation Properties
        public virtual Ticket Ticket { get; set; }

        //foriegn key
        [DisplayName("Ticket")]

        public virtual JTUser User { get; set; }

        //foriegn key
        [DisplayName("Team Member")]
        public string UserId { get; set; }
    }
}
