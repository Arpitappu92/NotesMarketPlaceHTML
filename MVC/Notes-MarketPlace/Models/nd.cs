using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace Notes_MarketPlace.Models
{
    public class nd
    {
        public SellerNotes note { get; set; }
        public NoteCategories Category { get; set; }
        public Country countryname { get; set; }
    }
}