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
    public class MailPrefference
    {
        [Display(Name = "Mail Prefference ID")]
        [Required(ErrorMessage = "You cannot leave the mail prefference id blank.")]
        public string ID { get; set; }

        [Display(Name = "Prefference")]
        [Required(ErrorMessage = "You cannot leave the mail prefference name blank.")]
        public string Name { get; set; }
    }
}
