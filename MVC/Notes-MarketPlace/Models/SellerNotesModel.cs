using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Notes_MarketPlace.Models
{
    public class SellerNotesModel
    {
        public int ID { get; set; }
        public int SellerID { get; set; }
        public int Status { get; set; }
        public Nullable<int> ActionedBy { get; set; }
        public string AdminRemarks { get; set; }
        public Nullable<System.DateTime> PublishedDate { get; set; }

        [StringLength(100, MinimumLength=3, ErrorMessage = "Enter NoteTitle between 3 to 100")]
        [Required(ErrorMessage ="Please Enter Note Title")]
        public string NoteTitle { get; set; }
        public int NoteCategory { get; set; }

       
        public HttpPostedFileBase DisplayPicture { get; set; }
        public int NoteType { get; set; }
        public Nullable<int> NumberOfPages { get; set; }
        [Required(ErrorMessage = "Please Enter Note Description")]
        public string NoteDescription { get; set; }
        public string UniversityInformation { get; set; }
        public Nullable<int> Country { get; set; }
        public string Course { get; set; }
        public string CourseCode { get; set; }
        public string ProfessorName { get; set; }

        [Required(ErrorMessage = "Please Select isPaid")]
        public bool isPaid { get; set; }
        public Nullable<int> SellPrice { get; set; }

        [Required(ErrorMessage = "Please Select A file to Upload")]
        public HttpPostedFileBase  PreviewUpload { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public bool IsActive { get; set; }


        [Required(ErrorMessage = "Please Select A file to Upload")]
        public HttpPostedFileBase UploadNote { get; set; }
    }
}