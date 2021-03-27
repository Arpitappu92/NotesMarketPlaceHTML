using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notes_MarketPlace.Models
{
        public class ProgressNote
        {
            public SellerNotes NoteData { get; set; }

            public NoteCategories Category { get; set; }

            public ReferenceData status { get; set; }
        }

    public class PublishNote
    {
        public SellerNotes NoteData { get; set; }

        public NoteCategories Category { get; set; }

        public ReferenceData status { get; set; }
    }

}