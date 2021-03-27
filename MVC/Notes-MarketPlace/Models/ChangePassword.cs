using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace Notes_MarketPlace.Models
{
    public class ChangePassword
    {

        [Display(Name = "Old Password")]
        [Required(ErrorMessage = "Please Enter Old Password")]
        /*[RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[#$^+=!*()@%&]).{6,24}$", ErrorMessage = "Invalid password format")]*/
        [DataType(DataType.Password)]
        /*[System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Does not match with Old Password")]*/
        public string OldPassword { get; set; }

        [Display(Name = "New Password")]
        [Required(ErrorMessage = "Please Enter New Password")]
/*        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[#$^+=!*()@%&]).{6,24}$", ErrorMessage = "Invalid password format")]*/
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "Please Enter  Confirm Password")]
        /*[RegularExpression("^(?=.[a-z])(?=.[A-Z])(?=.[0-9])(?=.[#$^+=!*()@%&]).{6,24}$", ErrorMessage = "Invalid password format")]*/
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "New Password And Confirm Password Does not match")]
        public string ConfirmPassword { get; set; }
    }
}