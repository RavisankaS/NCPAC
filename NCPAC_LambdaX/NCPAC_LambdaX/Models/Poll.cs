using NCPAC_LambdaX.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NCPAC_LambdaX.Models
{
    public class Poll
    {
        public Poll()
        {
            Options = new List<PollOption>();
            Votes = new List<PollVote>();
        }
        public int Id { get; set; }

        [Display(Name = "Poll Name")]
        [Required(ErrorMessage = "You cannot leave the poll name blank.")]
        [StringLength(50, ErrorMessage = "Name cannot be more than 50 characters long.")]
        public string Question { get; set; }

        [Display(Name = "Description")]
        [StringLength(500, ErrorMessage = "Poll description cannot be more than 500 characters long.")]
        public string Description { get; set; }

        [Display(Name = "Deadline")]
        public DateTime? TimeUntil { get; set; }

        [Display(Name = "Commitee")]
        public int? CommiteeID { get; set; }
        public Commitee? Commitee { get; set; }

        public List<PollOption> Options { get; set; }

        public virtual ICollection<PollVote> Votes { get; set; }
    }
}
