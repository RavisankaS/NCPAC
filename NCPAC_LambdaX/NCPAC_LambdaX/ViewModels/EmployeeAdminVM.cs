using NCPAC_LambdaX.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace NCPAC_LambdaX.ViewModels
{
    /// <summary>
    /// Add back in any Restricted Properties and list of UserRoles
    /// </summary>
    [ModelMetadataType(typeof(EmployeeMetaData))]
    public class EmployeeAdminVM : EmployeeVM
    {
        public string Email { get; set; }
        public bool Active { get; set; }

        [Display(Name = "Roles")]
        [Required(ErrorMessage = "You have to choose a role.")]
        public List<string> UserRoles { get; set; } = new List<string>();
    }
}
