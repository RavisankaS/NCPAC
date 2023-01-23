using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace NCPAC_LambdaX.ViewModels
{
    public class UserVM
    {
        public string Id { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "Roles")]
        public List<string> UserRoles { get; set; }
    }
}
