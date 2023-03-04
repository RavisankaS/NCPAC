using Microsoft.AspNetCore.Mvc;
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

    [ModelMetadataType(typeof(MemberMetaData))]
    public class Member : IValidatableObject
    {
        public int ID { get; set; }

        public string MemberName
        {
            get
            {
                string? mn = MiddleName;
                if (MiddleName == null)
                {
                    mn = "";
                }
                return Salutation
                    + " " + FirstName + " " + mn + " " + LastName;
            }
        }

        public string? HomeAddress
        {
            get
            {
                string? wc = City;
                string? wsa = StreetAddress;
                string? pc = PostalCodeFormatted;
                string? p = ProvinceID;
                if (WorkCity == null)
                {
                    wc = "";
                }
                if (WorkStreetAddress == null)
                {
                    wsa = "";
                }
                if (WorkPostalCodeFormatted == null)
                {
                    pc = "";
                }
                if (WorkProvinceID == null)
                {
                    p = "";
                }
                return wsa
                    + " " + wc + " " + p + " " + pc;

            }
        }

        public string? WorkAddress
        {
            get
            {
                string? wc = WorkCity;
                string? wsa = WorkStreetAddress;
                string? pc = WorkPostalCodeFormatted;
                string? p = WorkProvinceID;
                if (WorkCity == null)
                {
                    wc = "";
                }
                if (WorkStreetAddress == null)
                {
                    wsa = "";
                }
                if (WorkPostalCodeFormatted == null)
                {
                    pc = "";
                }
                if (WorkProvinceID == null)
                {
                    p = "";
                }
                return wsa
                    + " " + wc + " " + p + " " + pc;

            }
        }

        public string? PhoneFormatted
        {
            get
            {
                return "(" + Phone.Substring(0, 3) + ") " + Phone.Substring(3, 3) + "-" + Phone[6..];
            }
        }

        public string? WorkPhoneFormatted
        {
            get
            {
                if (WorkPhone != null)
                {

                    return "(" + WorkPhone.Substring(0, 3) + ") " + WorkPhone.Substring(3, 3) + "-" + WorkPhone[6..];

                }
                return "";

            }
        }


        public string? WorkPostalCodeFormatted
        {
            get
            {
                if (WorkPostalCode != null)
                {

                    return WorkPostalCode.Substring(0, 3) + " " + WorkPostalCode[3..];

                }
                return "";

            }
        }

        public string? PostalCodeFormatted
        {
            get
            {
                if (PostalCode != null)
                {

                    return PostalCode.Substring(0, 3) + " " + PostalCode[3..];

                }
                return "";

            }
        }

        public string FirstName { get; set; }

        public string? MiddleName { get; set; }

        public string LastName { get; set; }

        public string? Salutation { get; set; }

        public string? StreetAddress { get; set; }

        public string? City { get; set; }

        public string? ProvinceID { get; set; }
        public Province? Province { get; set; }


        public string? PostalCode { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? WorkStreetAddress { get; set; }

        public string? WorkCity { get; set; }

        public string? WorkProvinceID { get; set; }
        public Province? WorkProvince { get; set; }

        public string? WorkPostalCode { get; set; }

        public string? WorkPhone { get; set; }

        public string? WorkEmail { get; set; }

        public string? MailPrefferenceID { get; set; }
        public MailPrefference? MailPrefference { get; set; }

        public string? EducationalSummary { get; set; }

        public bool IsNCGrad { get; set; }

        public string? OccupationalSummary { get; set; }

        public DateTime? DateJoined { get; set; }

        public bool IsActive { get; set; }

        public ICollection<MemberCommitee> MemberCommitees { get; set; } = new HashSet<MemberCommitee>();

        public ICollection<ActionItem> ActionItems { get; set; } = new HashSet<ActionItem>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DateJoined > DateTime.Today)
            {
                yield return new ValidationResult("Date Joined cannot be in the future.", new[] { "DateJoined" });
            }
        }
    }
}
