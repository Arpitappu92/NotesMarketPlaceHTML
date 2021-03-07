using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Notes_MarketPlace.Models;


namespace Notes_MarketPlace.Controllers
{
    public class HomeController : Controller
    {
        private NotesEntities objNotesEntities = new NotesEntities();
        public ActionResult HomePage()
        {
            return View();
        }

        public ActionResult SearchNotes()
        {
            ViewBag.Country = objNotesEntities.Country.Where(x => x.IsActive == true);
            ViewBag.NoteType = objNotesEntities.NoteTypes.Where(x => x.IsActive == true);
            ViewBag.NoteCategory = objNotesEntities.NoteCategories.Where(x => x.IsActive == true);

            List<SellerNotes> sn = objNotesEntities.SellerNotes.ToList();
            return View(sn);
        }

        [HttpGet]
        public ActionResult ContactUs()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ContactUs(ContactUsModel obj)
        {
            if(ModelState.IsValid)
            {
                ContactUs cu = new ContactUs
                {
                    FullName = obj.FullName,
                    EmailId = obj.EmailId, 
                    Subject = obj.Subject,
                    Question = obj.Question,
                    CreatedDate = DateTime.Now,
                    IsActive = false
                };
                if(User.Identity.IsAuthenticated)
                {
                    cu.IsActive = true;
                }

                objNotesEntities.ContactUs.Add(cu);
                objNotesEntities.SaveChanges();
            }

            AccRelTemplate.ContactUsEmail.ContactUs(obj.Subject, obj.FullName, obj.Question);

            ModelState.Clear();

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}