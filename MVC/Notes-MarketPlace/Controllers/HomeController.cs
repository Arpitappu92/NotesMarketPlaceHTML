using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Notes_MarketPlace.Models;
using PagedList.Mvc;
using PagedList;

namespace Notes_MarketPlace.Controllers
{
    public class HomeController : Controller
    {
        private NotesEntities objNotesEntities = new NotesEntities();
        public ActionResult HomePage()
        {
            return View();
        }

        public ActionResult SearchNotes(string search, string NoteTp , string Catgry, string Univrsty, string Crs, string Cntry, int? pagesearch)
        {
            int id = Int16.Parse(Request.Cookies["ID"].Value);

            /*       ViewBag.Country = objNotesEntities.Country.Where(x => x.IsActive == true);
                   ViewBag.NoteType = objNotesEntities.NoteTypes.Where(x => x.IsActive == true);
                   ViewBag.NoteCategory = objNotesEntities.NoteCategories.Where(x => x.IsActive == true);
       */


            List<SellerNotes> NoteTitle = objNotesEntities.SellerNotes.OrderBy(x => x.NoteTitle).Where(x => x.IsActive == true && (x.NoteTitle.StartsWith(search) || search == null)).ToList();
            List<NoteCategories> CategoryName = objNotesEntities.NoteCategories.ToList();
            List<ReferenceData> StatusName = objNotesEntities.ReferenceData.ToList();
            List<Country> CountryName = objNotesEntities.Country.ToList();


            /*          var notes = objNotesEntities.SellerNotes.Where(x => x.NoteTitle.StartsWith(search) || search == null).ToList();
                      var cr = objNotesEntities.Country.ToList();

                      var seachedNotes = (from n in notes
                                          join c in cr on n.ID equals c.ID into table1
                                          from c in table1.ToList()
                                          select new nd
                                          {
                                              note = n,
                                              countryname = c
                                          }).ToList();

                      ViewBag.filterNotes = seachedNotes;

                     */
            var AllNotesRecords = from noti in NoteTitle
                                  join catnam in CategoryName on noti.NoteCategory equals catnam.ID into table1
                                  from catnam in table1.ToList()
                                  join stnam in StatusName on noti.Status equals stnam.ID into table2
                                  from stnam in table2.ToList().DefaultIfEmpty()
                                  join coname in CountryName on noti.Country equals coname.ID into table3
                                  from coname in table3.ToList().DefaultIfEmpty()
                                  where (stnam.Value == "Published" && ((noti.NoteCategory.ToString() == NoteTp || string.IsNullOrEmpty(NoteTp)) &&
                                  (noti.NoteType.ToString() == NoteTp || string.IsNullOrEmpty(NoteTp))
                                  && (noti.Country.ToString() == Cntry || string.IsNullOrEmpty(Cntry))
                                  && (noti.UniversityInformation == Univrsty || string.IsNullOrEmpty(Univrsty))
                                  && (noti.Course == Crs || string.IsNullOrEmpty(Crs))))
                                  select new AllSearchNotes
                                  {
                                      NoteDetails = noti,
                                      Category = catnam,
                                      status = stnam,
                                      Country = coname,
                                  };
            /*ViewBag.nd = objNotesEntities.SellerNotes.Where(x => x.NoteTitle.StartsWith(search) || search == null).ToList().Count();*/


            ViewBag.TotalNotesRecord = AllNotesRecords.Count();
            ViewBag.Ratings = objNotesEntities.SellerNotesReviews.Select(x => new { x.Ratings }).Distinct().ToList();
            ViewBag.Course = objNotesEntities.SellerNotes.Select(x => new { x.Course }).Distinct().Where(x => x.Course != null).ToList();
            ViewBag.University = objNotesEntities.SellerNotes.Select(x => new { x.UniversityInformation }).Distinct().Where(x => x.UniversityInformation != null).ToList();
            ViewBag.Country = new SelectList(objNotesEntities.Country, "ID", "CountryName");
            ViewBag.Category = new SelectList(objNotesEntities.NoteCategories, "ID", "CategoryName");
            ViewBag.NoteType = new SelectList(objNotesEntities.NoteTypes, "ID", "TypeName");

            /*List<SellerNotes> stnam = objNotesEntities.SellerNotes.ToList();*/

            return View(AllNotesRecords.ToList().ToPagedList(pagesearch ?? 1, 9));

        }

        [Route("FAQ")]
        public ActionResult FAQ()
        {
            return View();
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