using System.ComponentModel;

namespace JATS.Models
{
    public class Company
    {
        public int Id { get; set; }

        [DisplayName("Company Name")]
        public string Name { get; set; }

        [DisplayName("Company Description")]
        public string Description { get; set; }

        //Nav Props
        public virtual ICollection<JTUser> Members { get; set; }

        public virtual ICollection<Project> Projects { get; set; }

        public virtual ICollection<Invite> Invites { get; set; }


        ///
    }
}
