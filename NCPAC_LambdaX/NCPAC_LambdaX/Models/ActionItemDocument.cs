using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace NCPAC_LambdaX.Models
{
    public class ActionItemDocument : UploadedFile
    {
        [Display(Name = "Action Item")]
        public int ActionItemID { get; set; }

        public ActionItem ActionItem { get; set; }
    }
}
