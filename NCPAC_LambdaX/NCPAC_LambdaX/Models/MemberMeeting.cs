using NCPAC_LambdaX.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using NCPAC_LambdaX.ViewModels;

namespace NCPAC_LambdaX.Models
{
    public class MemberMeeting
    {
        public int MeetingID { get; set; }
        public Meeting Meeting { get; set; }

        public int MemberID { get; set; }
        public Member Member { get; set; }
    }
}
