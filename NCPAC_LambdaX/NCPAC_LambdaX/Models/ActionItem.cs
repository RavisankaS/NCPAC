using NCPAC_LambdaX.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NCPAC_LambdaX.Models
{
    public class ActionItem
    {
        public int ID { get; set; }

        [Display(Name = "Action Item Title")]
        [Required(ErrorMessage = "You cannot leave the Action Item Title blank.")]
        [StringLength(50, ErrorMessage = "Title cannot be more than 50 characters long.")]
        public string ActionItemTitle { get; set; }

        [Display(Name = "Description")]
        [StringLength(500, ErrorMessage = "Meeting description cannot be more than 500 characters long.")]
        public string Description { get; set; }

        [Display(Name = "Member")]
        [Required(ErrorMessage = "You have to choose a member.")]
        public int MemberID { get; set; }
        public Member Member { get; set; }

        [Display(Name = "Meeting")]
        [Required(ErrorMessage = "You have to choose a meeting.")]
        public int MeetingID { get; set; }
        public Meeting Meeting { get; set; }

        [Display(Name = "DateTime Appointed")]
        public DateTime? TimeAppointed { get; set; }

        [Display(Name = "Deadline")]
        public DateTime? TimeUntil { get; set; }

        [Display(Name = "Is Completed")]
        public bool IsCompleted { get; set; }


        [Display(Name = "Documents")]
        public ICollection<ActionItemDocument> ActionItemDocuments { get; set; } = new HashSet<ActionItemDocument>();

    }
}
