using NCPAC_LambdaX.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace NCPAC_LambdaX.ViewModels
{
    /// <summary>
    /// Leave out propeties that the Employee should not have access to change.
    /// </summary>
    public class PollViewModel
    {
        [Required]
        public string Question { get; set; }

        [Required]
        [MinLength(2)]
        public List<PollOption> OptionNames { get; set; }
    }

}
