using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JATS.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [DisplayName("Title")]
        public string Title { get; set; }

        [DisplayName("Ticket")]
        public int TicketId { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Date")]
        public DateTimeOffset Created { get; set; }

        [Required]
        [DisplayName("Recipient")]
        public string RecipientId { get; set; }

        [DisplayName("Has been viewed")]
        public bool Viewed { get; set; }

        [Required]
        [DisplayName("Sender")]
        public string SenderId { get; set; }


        //Nav Props
        public virtual Ticket Ticket { get; set; }
        public virtual JTUser Recipient { get; set; }

        public virtual JTUser Sender { get; set; }



        ///
    }
}
