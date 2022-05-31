using System.ComponentModel;

namespace JATS.Models
{
    public class TicketPriority
    {

        public int Id { get; set; }


        [DisplayName("Status Name")]
        public string Name { get; set; }
    }
}
