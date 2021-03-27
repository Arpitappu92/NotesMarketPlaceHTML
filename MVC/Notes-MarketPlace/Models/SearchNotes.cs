using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Notes_MarketPlace.Models;

namespace Notes_MarketPlace.Models
{ 
        public class AllSearchNotes
        {
            public SellerNotes NoteDetails { get; set; }

            public NoteCategories Category { get; set; }

            public ReferenceData status { get; set; }

            public Country Country { get; set; }

            public SellerNotesReviews Reviews { get; set; }

            public SellerNotesReportedIssues Reports { get; set; }
        }


    public class BuyerRequest
    {
        public Downloads DownloadNote { get; set; }

        public Users BuyerDetail { get; set; }

        public UserProfile UserProfileNote { get; set; }
    }

    public class MyDownloadNote
    {
        public Downloads DownloadNote { get; set; }

        public Users BuyerDetail { get; set; }

    }

    public class MySoldNote
    {
        public Downloads DownloadNote { get; set; }

        public Users BuyerDetail { get; set; }

    }


    public class RejectedNote
    {
        public SellerNotes NoteDetails { get; set; }

        public NoteCategories Category { get; set; }

        public ReferenceData status { get; set; }

    }
}