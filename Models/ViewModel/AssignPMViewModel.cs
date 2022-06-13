using Microsoft.AspNetCore.Mvc.Rendering;

namespace JATS.Models.ViewModel
{
    public class AssignPMViewModel
    {
        public Project Project { get; set; }
        public SelectList PMList { get; set; }
        public string PMid { get; set; }
    }
}
