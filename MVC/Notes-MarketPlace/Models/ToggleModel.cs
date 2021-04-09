using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notes_MarketPlace.Models
{
    public class ReportedIssues
    {
        public string Remarks { get; set; }
    }
    public class ReviewNote
    {
        public decimal Ratings { get; set; }
        public string Comments { get; set; }
    }
}