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
    public class MemberCommitee
    {
        public int MemberID { get; set; }
        public Member Member { get; set; }

        public int CommiteeID { get; set; }
        public Commitee Commitee { get; set; }
    }
}
