using Microsoft.AspNetCore.Mvc.Rendering;

namespace JATS.Models.ViewModel
{
    public class ManageUserRolesViewModel
    {

        public JTUser JTUser { get; set; }

        public MultiSelectList Roles { get; set; }

        public List<string> SelectedRoles { get; set; }
    }
}
