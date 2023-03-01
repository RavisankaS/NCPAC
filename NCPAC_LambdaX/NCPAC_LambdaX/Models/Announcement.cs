using NCPAC_LambdaX.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NCPAC_LambdaX.Models
{
    public class Announcement
    {
        public int ID { get; set; }

        [Display(Name = "Announcement Subject")]
        [Required(ErrorMessage = "You cannot leave the Subject blank.")]
        [StringLength(200, ErrorMessage = "Announcement subject name cannot be more than 200 characters long.")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "You cannot leave the  Announcement blank.")]
        [StringLength(5000, ErrorMessage = "Announcement cannot be more than 5000 characters long.")]
        public string AnnouncementDescription { get; set; }

        [Display(Name = "Time Posted")]
        public DateTime? TimePosted { get; set; }

    }
}
