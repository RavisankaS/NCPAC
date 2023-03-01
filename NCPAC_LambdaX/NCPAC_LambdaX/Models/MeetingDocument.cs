using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace NCPAC_LambdaX.Models
{
    public class MeetingDocument : UploadedFile
    {
        [Display(Name = "Meeting")]
        public int MeetingID { get; set; }

        public Meeting Meeting { get; set; }
    }
}
