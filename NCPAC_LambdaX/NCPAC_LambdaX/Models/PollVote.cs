using NCPAC_LambdaX.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NCPAC_LambdaX.Models
{
    public class PollVote
    {
        public int Id { get; set; }

        public int PollId { get; set; }

        public string UserId { get; set; }

        [Required]
        public int SelectedOption { get; set; }

        public virtual Poll Poll { get; set; }
    }
}
