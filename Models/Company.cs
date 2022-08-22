using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JATS.Models
{
    public class Company
    {
        public int Id { get; set; }

        [DisplayName("Company Name")]
        public string Name { get; set; }

        [DisplayName("Company Description")]
        public string Description { get; set; }

        [DisplayName("PhoneNumber")]
        public string? PhoneNumber { get; set; }

        [DisplayName("Email Address")]
        public string? Email { get; set; }

        [DisplayName("Facebook")]
        public string? SocialMediaFacebook { get; set; }

        [DisplayName("Instagram")]
        public string? SocialMediaInstagram { get; set; }

        [DisplayName("Twitter")]
        public string? SocialMediaTwitter { get; set; }

        //File upload
        [NotMapped]
        [DataType(DataType.Upload)]
        public IFormFile CompanyBGFormFile { get; set; }

        [DisplayName("Company Background")]
        public string? CompanyBGFileName { get; set; }
        public byte[]? CompanyBG { get; set; }

        [DisplayName("File Extension")]
        public string? CompanyBGFileContentType { get; set; }



        //Nav Props
        public virtual ICollection<JTUser> Members { get; set; }

        public virtual ICollection<Project> Projects { get; set; }

        public virtual ICollection<Invite> Invites { get; set; }


        ///
    }
}
