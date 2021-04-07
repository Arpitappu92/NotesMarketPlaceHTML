//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Notes_MarketPlace.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SellerNotes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SellerNotes()
        {
            this.Downloads = new HashSet<Downloads>();
            this.SellerNotesAttachements = new HashSet<SellerNotesAttachements>();
            this.SellerNotesReportedIssues = new HashSet<SellerNotesReportedIssues>();
            this.SellerNotesReviews = new HashSet<SellerNotesReviews>();
        }
    
        public int ID { get; set; }
        public int SellerID { get; set; }
        public int Status { get; set; }
        public Nullable<int> ActionedBy { get; set; }
        public string AdminRemarks { get; set; }
        public Nullable<System.DateTime> PublishedDate { get; set; }
        public string NoteTitle { get; set; }
        public int NoteCategory { get; set; }
        public string DisplayPicture { get; set; }
        public int NoteType { get; set; }
        public Nullable<int> NumberOfPages { get; set; }
        public string NoteDescription { get; set; }
        public string UniversityInformation { get; set; }
        public Nullable<int> Country { get; set; }
        public string Course { get; set; }
        public string CourseCode { get; set; }
        public string ProfessorName { get; set; }
        public bool isPaid { get; set; }
        public Nullable<int> SellPrice { get; set; }
        public string PreviewUpload { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public bool IsActive { get; set; }
    
        public virtual Country Country1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Downloads> Downloads { get; set; }
        public virtual NoteCategories NoteCategories { get; set; }
        public virtual NoteTypes NoteTypes { get; set; }
        public virtual ReferenceData ReferenceData { get; set; }
        public virtual Users Users { get; set; }
        public virtual Users Users1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SellerNotesAttachements> SellerNotesAttachements { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SellerNotesReportedIssues> SellerNotesReportedIssues { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SellerNotesReviews> SellerNotesReviews { get; set; }
    }
}