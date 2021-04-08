using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notes_MarketPlace.Models
{
    public class SpamReportsmodel
    {

        public SellerNotesReportedIssues Reports { get; set; }
        public SellerNotes NoteDetails { get; set; }
        public NoteCategories Category { get; set; }
        public Users user { get; set; }
    }

}