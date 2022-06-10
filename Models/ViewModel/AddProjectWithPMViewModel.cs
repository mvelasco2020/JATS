using Microsoft.AspNetCore.Mvc.Rendering;

namespace JATS.Models.ViewModel
{
    public class AddProjectWithPMViewModel
    {

        public Project Project { get; set; }
        public SelectList PMList { get; set; }

        public string PMid { get; set; }

        public SelectList PriorityList { get; set; }

        public int ProjectPriority { get; set; }

    }
}
