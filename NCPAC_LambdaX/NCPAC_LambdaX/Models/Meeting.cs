using NCPAC_LambdaX.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NCPAC_LambdaX.Models
{
    public class Meeting 
    {
        public int ID { get; set; }

        [Display(Name = "Meeting Title")]
        [Required(ErrorMessage = "You cannot leave the Meeting Title blank.")]
        [StringLength(50, ErrorMessage = "Meeting title cannot be more than 50 characters long.")]
        public string MeetingTitle { get; set; }

        [Display(Name = "Description")]
        [StringLength(500, ErrorMessage = "Meeting description cannot be more than 500 characters long.")]
        public string Description { get; set; }

        [Display(Name = "Meeting Link")]
        [StringLength(1000, ErrorMessage = "Meeting Link cannot be more than 1000 characters long.")]
        public string? MeetingLink { get; set; }

        [Display(Name = "From")]
        public DateTime? TimeFrom { get; set; }

        [Display(Name = "To")]
        public DateTime? TimeTo { get; set; }

        [Display(Name = "Commitees")]
        public ICollection<MeetingCommitee> MeetingCommitees { get; set; } = new HashSet<MeetingCommitee>();

        [Display(Name = "Is Complete")]
        public bool IsArchived { get; set; }

        [Display(Name = "Cancelled")]
        public bool IsCancelled { get; set; }


        [Display(Name = "Documents")]
        public ICollection<MeetingDocument> MeetingDocuments { get; set; } = new HashSet<MeetingDocument>();

    }
}
