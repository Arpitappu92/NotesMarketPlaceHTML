using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notes_MarketPlace.Models
{
    public class UserProfileModel
    {
        public int UserID { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string PhoneCountryCode { get; set; }
        public string PhoneNumber { get; set; }
        public HttpPostedFileBase ProfilePicture { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public int Country { get; set; }
        public string University { get; set; }
        public string College { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }

    }
}