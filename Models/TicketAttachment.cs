﻿using System.ComponentModel;
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
        public string UserId { get; set; }

        [DisplayName("Ticket")]
        public int TicketId { get; set; }

        [DisplayName("File Description")]
        public string Description { get; set; }

        //File upload
        [NotMapped]
        [DataType(DataType.Upload)]
        public IFormFile FormFile { get; set; }

        [DisplayName("File Name")]
        public string FileName { get; set; }
        public byte[] Data { get; set; }

        [DisplayName("File Extension")]
        public string FileContentType { get; set; }


        //Navigation Properties
        public virtual Ticket Ticket { get; set; }
        public virtual JTUser User { get; set; }
    }
}
