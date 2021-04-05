using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Notes_MarketPlace.Models
{
    public class UpdateFrontUserProfile
    {

        public int ID { get; set; }

        public int UserID { get; set; }


        [Required(ErrorMessage = "Please Enter Your First Name")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "only alphabet allowed !")]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "Please Enter Your Last Name")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "only alphabet allowed !")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please Enter EmailID")]
        [EmailAddress(ErrorMessage = "Enter valid email address")]
        public string EmailID { get; set; }

        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string PhoneCountryCode { get; set; }
        [Required(ErrorMessage = "Please Enter Phone Number")]
        [RegularExpression("^[0-9]{1,12}$", ErrorMessage = "Only Numerics Allowed with max 12 !")]

        public string PhoneNumber { get; set; }
        public HttpPostedFileBase ProfilePicture { get; set; }
        [Required(ErrorMessage = "Please Enter AddressLine1")]

        public string AddressLine1 { get; set; }
        [Required(ErrorMessage = "Please Enter AddressLine2")]

        public string AddressLine2 { get; set; }


        [Required(ErrorMessage = "Please Enter Phone City")]

        public string City { get; set; }
        [Required(ErrorMessage = "Please Enter State")]

        public string State { get; set; }
        [Required(ErrorMessage = "Please Enter Zip Code")]
        [RegularExpression("^[0-9]{1,8}$", ErrorMessage = "Only Numerics Allowed with max 8 !")]

        public string ZipCode { get; set; }
        public int Country { get; set; }
        public string University { get; set; }
        public string College { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }

        public virtual Country Country1 { get; set; }
        public virtual Users Users { get; set; }
    }


    public class AdminUserProfileModel
    {
        public int ID { get; set; }

        public int UserID { get; set; }

        [Required(ErrorMessage = "Please Enter Your First Name")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "only alphabet allowed !")]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "Please Enter Your Last Name")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "only alphabet allowed !")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please Enter EmailID")]
        [EmailAddress(ErrorMessage = "Enter valid email address")]
        public string EmailID { get; set; }

        public string SecondaryEmail { get; set; }
        public string PhoneCountryCode { get; set; }
        public string PhoneNumber { get; set; }
        public HttpPostedFileBase ProfilePicture { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }

    }
}