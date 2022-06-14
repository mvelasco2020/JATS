using Microsoft.AspNetCore.Mvc.Rendering;

namespace JATS.Models.ViewModel
{
    public class ProjectMembersViewModel
    {
        public Project Project { get; set; }

        public MultiSelectList Users { get; set; }

        public List<String> SelectedUsers { get; set; }

    }
}
