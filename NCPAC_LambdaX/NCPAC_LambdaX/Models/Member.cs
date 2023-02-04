using NCPAC_LambdaX.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NCPAC_LambdaX.Models
{
    public class Member
    {
        public int ID { get; set; }

        [Display(Name = "Member")]
        public string MemberName
        {
            get
            {
                return Salutation
                    + " " + FirstName + " " + MiddleName + " " + LastName;
            }
        }

        [Display(Name = "Home Address")]
        public string HomeAddress
        {
            get
            {
                return StreetAddress
                    + " " + City + " " + PostalCode;
            }
        }

        [Display(Name = "Work Address")]
        public string WorkAddress
        {
            get
            {
                return WorkStreetAddress
                    + " " + WorkCity + " " + WorkPostalCode;
            }
        }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "You cannot leave the first name empty.")]
        [StringLength(30, ErrorMessage = "First name cannot be more than 30 characters long.")]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        [StringLength(30, ErrorMessage = "Middle name cannot be more than 30 characters long.")]
        public string? MiddleName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "You cannot leave the last name empty.")]
        [StringLength(50, ErrorMessage = "Last name cannot be more than 50 characters long.")]
        public string LastName { get; set; }

        [Display(Name = "Salutation")]
        public string? Salutation { get; set; }

        [Display(Name = "Street Adress")]
        [StringLength(200, ErrorMessage = "Street Adress cannot be more than 200 characters long.")]
        public string? StreetAddress { get; set; }

        [Display(Name = "City")]
        [StringLength(100, ErrorMessage = "City cannot be more than 100 characters long.")]
        public string? City { get; set; }

        [Display(Name = "Province")]
        public string? ProvinceID { get; set; }
        public Province? Province { get; set; }

        [StringLength(6, ErrorMessage = "Please enter a valid 6 character Postal code with no spaces.")]
        [DataType(DataType.PostalCode)]
        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }

        [RegularExpression("^\\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number (without spaces).")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(10)]
        [Display(Name = "Home Phone")]
        public string? Phone { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid email adress.")]
        [Display(Name = "Personal Email")]
        public string? Email { get; set; }

        [Display(Name = "Work Street Adress")]
        [StringLength(200, ErrorMessage = "Street Adress cannot be more than 200 characters long.")]
        public string? WorkStreetAddress { get; set; }

        [Display(Name = "Work City")]
        [StringLength(100, ErrorMessage = "City cannot be more than 100 characters long.")]
        public string? WorkCity { get; set; }

        [Display(Name = "Work Province")]
        public string? WorkProvinceID { get; set; }
        public Province? WorkProvince { get; set; }

        [StringLength(6, ErrorMessage = "Please enter a valid 6 character Postal code with no spaces.")]
        [DataType(DataType.PostalCode)]
        [Display(Name = "Work Postal Code")]
        public string? WorkPostalCode { get; set; }

        [RegularExpression("^\\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number (without spaces).")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(10)]
        [Display(Name = "Work Phone")]
        public string? WorkPhone { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid email adress.")]
        [Display(Name = "Work Email")]
        public string? WorkEmail { get; set; }

        [Display(Name = "Preffered Email")]
        public string? PrefferedEmail { get; set; }

        [Display(Name = "Educational Summary")]
        public string? EducationalSummary { get; set; }

        [Display(Name = "Is a Niagara College Graduate")]
        public bool IsNCGrad { get; set; }

        [Display(Name = "Occupational Summary")]
        public string? OccupationalSummary { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Joined")]
        public DateTime? DateJoined { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Commitees")]
        public ICollection<MemberCommitee> MemberCommitees { get; set; } = new HashSet<MemberCommitee>();

    }
}
