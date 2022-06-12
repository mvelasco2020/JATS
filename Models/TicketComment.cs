using System.ComponentModel;

namespace JATS.Models
{
    public class TicketComment
    {

        public int Id { get; set; }


        [DisplayName("Member Comment")]
        public string Comment { get; set; }

        [DisplayName("Date")]
        public DateTimeOffset Created { get; set; }

        [DisplayName("Ticket")]
        public int TicketId { get; set; }
        public virtual Ticket? Ticket { get; set; }

        [DisplayName("Team Member")]
        public string UserId { get; set; }
        public virtual JTUser? User { get; set; }




    }
}
