using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JATS.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("Title")]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        [DisplayName("Description")]
        public string Description { get; set; }



        [DataType(DataType.Date)]
        [DisplayName("Created")]
        public DateTimeOffset Created { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Updated")]
        public DateTimeOffset? Updated { get; set; }


        [DisplayName("Archived")]
        public bool Archived { get; set; }

        [DisplayName("Project")]
        public int ProjectId { get; set; }


        [DisplayName("Ticket Type")]
        public int TicketTypeId { get; set; }

        [DisplayName("Ticket Priority")]
        public int TicketPriorityId { get; set; }

        [DisplayName("Ticket Status")]
        public int TicketStatusId { get; set; }

        [DisplayName("Ticket Owner")]
        public string? OwnerUserId { get; set; }

        [DisplayName("Ticket Technician")]
        public string? TechnicianUserId { get; set; }

        //Nav Props
        public virtual Project Project { get; set; }

        public virtual TicketType TicketType { get; set; }

        public virtual TicketPriority TicketPriority { get; set; }

        public virtual TicketStatus TicketStatus { get; set; }

        public virtual JTUser OwnerUser { get; set; }
        public virtual JTUser TechnicianUser { get; set; }

        public virtual ICollection<TicketComment> Comments { get; set; }

        public virtual ICollection<TicketAttachment> Attachments { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; }

        public virtual ICollection<TicketHistory> History { get; set; }





    }
}
