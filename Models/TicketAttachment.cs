using JATS.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JATS.Models
{
    public class TicketAttachment
    {
        public int Id { get; set; }

        [DisplayName("File Date")]
        public DateTimeOffset Created { get; set; }

        [DisplayName("Team Member")]
        [ValidateNever]
        public string UserId { get; set; }

        [DisplayName("Ticket")]
        [ValidateNever]
        public int TicketId { get; set; }

        [DisplayName("File Description")]
        [Required]
        public string Description { get; set; }

        //File upload
        [NotMapped]
        [DisplayName("Select a file")]
        [DataType(DataType.Upload)]
        [MaxFileSize(1024 * 1024)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".doc", ".docx", ".xls", ".xlsx", ".pdf" })]
        [ValidateNever]
        public IFormFile FormFile { get; set; }

        [DisplayName("File Name")]
        [ValidateNever]
        public string FileName { get; set; }

        [ValidateNever]
        public byte[] Data { get; set; }

        [DisplayName("File Extension")]
        [ValidateNever]
        public string FileContentType { get; set; }


        //Navigation Properties
        [ValidateNever]
        public virtual Ticket Ticket { get; set; }
        [ValidateNever]
        public virtual JTUser User { get; set; }
    }
}
