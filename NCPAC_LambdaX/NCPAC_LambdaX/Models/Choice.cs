using NCPAC_LambdaX.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NCPAC_LambdaX.Models
{
    public class Choice
    {
        public int ID { get; set; }

        [Display(Name = "Choice")]
        [Required(ErrorMessage = "You cannot leave the choice name blank.")]
        [StringLength(50, ErrorMessage = "Title cannot be more than 50 characters long.")]
        public string Name { get; set; }

        [Display(Name = "Poll")]
        [Required(ErrorMessage = "You have to choose a poll.")]
        public int PollID { get; set; }
        public Poll Poll { get; set; }

    }
}
