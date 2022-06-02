using System.ComponentModel;

namespace JATS.Models
{
    public class ProjectPriority
    {

        public int Id { get; set; }
        [DisplayName("Project Priority")]
        public string Name { get; set; }
    }
}
