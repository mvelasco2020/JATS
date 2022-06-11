using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JATS.Models
{
    public class Project
    {

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Project Name")]
        public string Name { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }


        [DisplayName("Start Date")]
        [DataType(DataType.Date)]
        public DateTimeOffset StartDate { get; set; }

        [DisplayName("End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public bool Archived { get; set; }

        //Image
        [NotMapped]
        [DataType(DataType.Upload)]
        public IFormFile? ImageFormFile { get; set; }

        [DisplayName("File Name")]
        public string? ImageFileName { get; set; }
        public byte[]? ImageFileData { get; set; }

        [DisplayName("File Extension")]
        public string? FileContentType { get; set; }

        [DisplayName("Company")]
        public int? CompanyId { get; set; }

        [DisplayName("Priority")]
        public int? ProjectPriorityId { get; set; }


        //Nav Props

        public virtual Company Company { get; set; }

        public virtual ProjectPriority ProjectPriority { get; set; }

        public virtual ICollection<JTUser> Members { get; set; } = new HashSet<JTUser>();

        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();

        ///

    }
}
