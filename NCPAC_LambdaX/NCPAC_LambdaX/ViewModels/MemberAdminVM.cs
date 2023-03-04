using NCPAC_LambdaX.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace NCPAC_LambdaX.ViewModels
{
    /// <summary>
    /// Add back in any Restricted Properties and list of UserRoles
    /// </summary>
    [ModelMetadataType(typeof(MemberMetaData))]
    public class MemberAdminVM : MemberVM
    {
        public DateTime? DateJoined { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Roles")]
        public List<string> UserRoles { get; set; } = new List<string>();

        [Display(Name = "Action Items")]
        public ICollection<ActionItem> ActionItems { get; set; } = new HashSet<ActionItem>();
    }
}
