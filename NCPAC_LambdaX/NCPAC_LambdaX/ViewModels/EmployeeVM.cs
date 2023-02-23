using NCPAC_LambdaX.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace NCPAC_LambdaX.ViewModels
{
    /// <summary>
    /// Leave out propeties that the Employee should not have access to change.
    /// </summary>
    [ModelMetadataType(typeof(EmployeeMetaData))]
    public class EmployeeVM
    {
        public int ID { get; set; }
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
        public string FormalName
        {
            get
            {
                return LastName + ", " + FirstName;
            }
        }
        public string PhoneNumber
        {
            get
            {
                if (String.IsNullOrEmpty(Phone))
                {
                    return "";
                }
                else
                {
                    return "(" + Phone.Substring(0, 3) + ") " + Phone.Substring(3, 3) + "-" + Phone.Substring(6, 4);
                }
            }
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
    }

}
