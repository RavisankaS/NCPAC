using NCPAC_LambdaX.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NCPAC_LambdaX.Models
{
    public class PollOption
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int PollId { get; set; }
        public Poll Poll { get; set; }
        public int VoteCount { get; set; }
    }
}
