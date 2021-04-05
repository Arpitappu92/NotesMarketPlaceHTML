using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Notes_MarketPlace.Models
{
    public class UserProfileModel
    {
        public int UserID { get; set; }

        [Required(ErrorMessage = "Please Enter DOB")]

        public Nullable<System.DateTime> DateOfBirth { get; set; }
        [Required(ErrorMessage = "Please Enter Gender")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Please Enter CountryCode")]
        public string PhoneCountryCode { get; set; }

        [Required(ErrorMessage = "Please Enter PhoneNo.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please Upload a file")]
        public HttpPostedFileBase ProfilePicture { get; set; }

        [Required(ErrorMessage = "Please Enter AddressLine1")]
        public string AddressLine1 { get; set; }

        [Required(ErrorMessage = "Please Enter AddressLine2")]
        public string AddressLine2 { get; set; }

        [Required(ErrorMessage = "Please Enter City")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please Enter State")]
        public string State { get; set; }

        [Required(ErrorMessage = "Please Enter ZipCode")]
        public string ZipCode { get; set; }
        public int Country { get; set; }
        public string University { get; set; }
        public string College { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }

    }

}