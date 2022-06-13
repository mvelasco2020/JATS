using Microsoft.AspNetCore.Mvc.Rendering;

namespace JATS.Models.ViewModel
{
    public class AssignTechnicianViewModel
    {
        public Ticket Ticket { get; set; }

        public SelectList Technicians { get; set; }

        public string TechnicianId { get; set; }
    }
}
