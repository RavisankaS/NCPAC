using NCPAC_LambdaX.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NCPAC_LambdaX.Models
{
    public class Province
    {
        [Display(Name = "Two Letter Province Code")]
        [Required(ErrorMessage = "You cannot leave the province code blank.")]
        [StringLength(2, ErrorMessage = "Province Code can only be two capital letters.")]
        [RegularExpression("^\\p{Lu}{2}$", ErrorMessage = "Please enter two capital letters for the province code.")]
        public string ID { get; set; }

        [Display(Name = "Province Name")]
        [Required(ErrorMessage = "You cannot leave the name of the province blank.")]
        [StringLength(50, ErrorMessage = "Province name can only be 50 characters long.")]
        public string Name { get; set; }
    }
}
