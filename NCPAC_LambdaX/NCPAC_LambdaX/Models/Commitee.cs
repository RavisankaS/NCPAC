using NCPAC_LambdaX.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NCPAC_LambdaX.Models
{
    public class Commitee
    {
        public int ID { get; set; }

        [Display(Name = "Commitee Name")]
        [Required(ErrorMessage = "You cannot leave the Commitee name blank.")]
        [StringLength(50, ErrorMessage = "Commitee name cannot be more than 50 characters long.")]
        public string CommiteeName { get; set; }

        [Required(ErrorMessage = "You cannot leave the  Academic Division blank.")]
        [StringLength(50, ErrorMessage = "Division name cannot be more than 50 characters long.")]
        public string Division { get; set; }

        [Display(Name = "Members")]
        public ICollection<MemberCommitee> MemberCommitees { get; set; } = new HashSet<MemberCommitee>();

        [Display(Name = "Meetings")]
        public ICollection<MeetingCommitee> MeetingCommitees { get; set; } = new HashSet<MeetingCommitee>();

    }
}
