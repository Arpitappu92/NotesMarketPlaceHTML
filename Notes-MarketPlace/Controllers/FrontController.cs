using Notes_MarketPlace.Models;
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Net;

namespace Notes_MarketPlace.Controllers
{

    [Authorize]
    [RoutePrefix("Front")]
    public class FrontController : Controller
    {
        private NotesEntities objNotesEntities = new NotesEntities();

        [Route("Dashboard")]
        // GET: Front
        public ActionResult Dashboard(string Sorting_Order, string SortOrder, string Search_Data ,string Search_second)
        {
            int id = Int16.Parse(Request.Cookies["ID"].Value);
            /*
                             ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "Name_Description" : "";
            */
            ViewBag.DateSorted = string.IsNullOrEmpty(Sorting_Order) ? "CreatedDate_asc" : "";
            ViewBag.TitleSorted= Sorting_Order == "Title" ? "Title_desc" : "Title";
            ViewBag.CategorySorted = Sorting_Order == "Category" ? "Category_desc" : "Category";
            ViewBag.StatusSorted = Sorting_Order == "Status" ? "Status_desc" : "Status";
            /* List<SellerNotes> sn = objNotesEntities.SellerNotes.ToList();
 */
            /*var note = objNotesEntities.SellerNotes.ToList().Where(x => x.SellerID == id && x.IsActive == true);
            ViewBag.NoteData = note;
*/
            List<SellerNotes> NoteTitle = objNotesEntities.SellerNotes.OrderByDescending(x => x.CreatedDate).Where(x => x.CreatedBy == id && x.IsActive == true && (x.NoteTitle.Contains(Search_Data) || Search_Data == null)).ToList();
            List<NoteCategories> CategoryName = objNotesEntities.NoteCategories.ToList();
            List<ReferenceData> StatusName = objNotesEntities.ReferenceData.Where(x => x.RefCategory == "Note Status" && x.Value != "Rejected" && x.Value != "Removed" && x.IsActive == true).ToList();

            var note = from x in objNotesEntities.SellerNotes select x;
                /*{
                    sn = sn.Where(x => x.NoteTitle.ToUpper().Contains(Search_Data.ToUpper())
                        || x.NoteCategories.CategoryName.ToUpper().Contains(Search_Data.ToUpper()));
                }
                {
                    sn = sn.Where(x => x.NoteTitle.ToUpper().Contains(Search_second.ToUpper())
                        || x.NoteCategories.CategoryName.ToUpper().Contains(Search_second.ToUpper()));
                }
                switch (Sorting_Order)
                {
                    case "Name_Description":
                        sn = sn.OrderByDescending(x => x.NoteTitle);
                        break;

                    default:
                        sn = sn.OrderBy(x => x.NoteTitle);
                        break;
                }*/
            /*  var notesinstate= (from nt in NoteTitle
                                  join cn in CategoryName on nt.NoteCategory equals cn.ID into table1
                                  from cn in table1.ToList()
                                  join sn in StatusName on nt.Status equals sn.ID into table2
                                  from sn in table2.ToList()
                                  where sn.Value != "Published"
                                  select new InProgressNote
                                  {
                                      NoteDetails = nt,
                                      Category = cn,
                                      status = sn
                                  }).AsQueryable();*/

            var sn = objNotesEntities.SellerNotes.ToList();

            switch (SortOrder)
            {
                case "CreatedDate_asc":
                    note=note.OrderBy(x => x.CreatedDate);
                    break;
                case "Title_desc":
                    note=note.OrderByDescending(x => x.NoteTitle);
                    break;
                case "Title":
                    note=note.OrderBy(x => x.NoteTitle);
                    break;
                case "Category_desc":
                    note=note.OrderByDescending(x => x.NoteCategories.CategoryName);
                    break;
                case "Category":
                    note=note.OrderBy(x => x.NoteCategories.CategoryName);
                    break;
                case "Status_desc":
                    note=note.OrderByDescending(x => x.Status);
                    break;
                case "Status":
                    note=note.OrderBy(x => x.Status);
                    break;
                default:
                    note=note.OrderByDescending(x => x.CreatedDate);
                    break;
            }
            ViewBag.note = note.ToList();


            /*   {
                   return View(objNotesEntities.SellerNotes.Where(x => x.NoteTitle.StartsWith(search) || search == null).ToList());
               }
   */
            return View();
        }

        [Route("AddNotes")]
        [HttpGet]
        public ActionResult AddNotes()
        {

            ViewBag.Country = objNotesEntities.Country.Where(x => x.IsActive == true);
            ViewBag.NoteType = objNotesEntities.NoteTypes.Where(x => x.IsActive == true);
            ViewBag.NoteCategory = objNotesEntities.NoteCategories.Where(x => x.IsActive == true);

            return View();
        }


        [HttpPost]
        public ActionResult AddNotes(SellerNotesModel obj)
        {
            if (ModelState.IsValid)
            {
                int id = Int16.Parse(Request.Cookies["ID"].Value);

                SellerNotes note = new SellerNotes
                {
                    SellerID = id,
                    Status = 8,
                    ActionedBy = 22,
                    AdminRemarks = "Rejected",
                    PublishedDate = DateTime.Now,
                    NoteTitle = obj.NoteTitle,
                    NoteCategory = obj.NoteCategory,
                    NoteType = obj.NoteType,
                    NumberOfPages = obj.NumberOfPages,
                    NoteDescription = obj.NoteDescription,
                    UniversityInformation = obj.UniversityInformation,
                    Country = obj.Country,
                    Course = obj.Course,
                    CourseCode = obj.CourseCode,
                    ProfessorName = obj.ProfessorName,
                    isPaid = obj.isPaid,
                    SellPrice = obj.SellPrice,
                    CreatedDate = DateTime.Now,
                    IsActive = true

                };

                objNotesEntities.SellerNotes.Add(note);
                objNotesEntities.SaveChanges();

                var sellernoteid = note.ID;

                string path = Path.Combine(Server.MapPath("~/Members/" + Request.Cookies["ID"].Value), sellernoteid.ToString());
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (obj.DisplayPicture != null && obj.DisplayPicture.ContentLength > 0)
                {
                    string fileName = Path.GetFileNameWithoutExtension(obj.DisplayPicture.FileName);
                    string extension = Path.GetExtension(obj.DisplayPicture.FileName);
                    fileName = "DP_" + DateTime.Now.ToString("ddMMyyyy") + extension;
                    string finalpath = Path.Combine(path, fileName);
                    obj.DisplayPicture.SaveAs(finalpath);

                    note.DisplayPicture = Path.Combine(("~/Members/" + Request.Cookies["ID"].Value + "/" + sellernoteid.ToString() + "/"), fileName);
                    objNotesEntities.SaveChanges();
                }


                if (obj.PreviewUpload != null && obj.PreviewUpload.ContentLength > 0)
                {
                    string fileName = Path.GetFileNameWithoutExtension(obj.PreviewUpload.FileName);
                    string extension = Path.GetExtension(obj.PreviewUpload.FileName);
                    fileName = "Preview_" + DateTime.Now.ToString("ddMMyyyy") + extension;
                    string finalpath = Path.Combine(path, fileName);
                    obj.PreviewUpload.SaveAs(finalpath);

                    note.PreviewUpload = Path.Combine(("~/Members/" + Request.Cookies["ID"].Value + "/" + note.ID.ToString() + "/"), fileName);
                    objNotesEntities.SaveChanges();
                }


                SellerNotesAttachements sna = new SellerNotesAttachements();
                sna.NoteID = note.ID;
                sna.CreatedDate = DateTime.Now;
                sna.IsActive = true;


                string storepath = Path.Combine(Server.MapPath("~/Members/" + Request.Cookies["ID"].Value + "/" + sna.NoteID.ToString()), "attachments");
                if (!Directory.Exists(storepath))
                {
                    Directory.CreateDirectory(storepath);
                }


                if (obj.UploadNote != null && obj.UploadNote.ContentLength > 0)
                {
                    string fileName = Path.GetFileNameWithoutExtension(obj.PreviewUpload.FileName);
                    string extension = Path.GetExtension(obj.PreviewUpload.FileName);
                    fileName = sna.ID + "_" + DateTime.Now.ToString("ddMMyyyy") + extension;
                    string finalpath = Path.Combine(storepath, fileName);
                    obj.UploadNote.SaveAs(finalpath);

                    sna.FileName = fileName;

                    sna.FilePath = Path.Combine(("~/Members/" + Request.Cookies["ID"].Value + "/" + sna.NoteID.ToString() + "/attachments"), fileName);
                    objNotesEntities.SaveChanges();
                }
                objNotesEntities.SellerNotesAttachements.Add(sna);
                objNotesEntities.SaveChanges();


            }
            ModelState.Clear();
            return RedirectToAction("AddNotes", "Front");
        }


        [AllowAnonymous]
        [Route("NoteDetails/{id}")]
        public ActionResult NoteDetails(int? id)
        {



            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SellerNotes sellernote = objNotesEntities.SellerNotes.Find(id);
            if(sellernote == null)
            {
                return HttpNotFound();
            }

            var data = objNotesEntities.SellerNotes.Where(x => x.ID == id).FirstOrDefault();
            ViewBag.dispic = data.DisplayPicture;
            ViewBag.notepre = data.PreviewUpload;
      
            /*   Country con = objNotesEntities.Country.Find(sellernote.Country);
               NoteCategories cat = objNotesEntities.NoteCategories.Find(sellernote.NoteCategories);
               SellerNotesAttachements sna = (SellerNotesAttachements)objNotesEntities.SellerNotesAttachements.Where(x => x.NoteID == sellernote.ID);
   */
            return View(data);
        }








        public ActionResult BuyerRequest()
        {
            
            return View();
        }

















        [HttpGet]
        public ActionResult UserProfile()
        {
            ViewBag.Country = objNotesEntities.Country.Where(x => x.IsActive == true);
            return View();
        }

        [HttpPost]
        public ActionResult UserProfile(UserProfileModel obj)
        {
            int id = Int16.Parse(Request.Cookies["ID"].Value);

            

            if (ModelState.IsValid)
            {

                UserProfile uprof = new UserProfile
                {
                    UserID = id,
                    DateOfBirth = obj.DateOfBirth,
                    Gender = obj.Gender,
                    PhoneCountryCode = obj.PhoneCountryCode,
                    PhoneNumber = obj.PhoneNumber,
                    AddressLine1 = obj.AddressLine1,
                    AddressLine2 = obj.AddressLine2,
                    City = obj.City,
                    State = obj.State,
                    ZipCode = obj.ZipCode,
                    Country = obj.Country,
                    University = obj.University,
                    College = obj.College,
                    CreatedDate = DateTime.Now
                };
                objNotesEntities.UserProfile.Add(uprof);
                objNotesEntities.SaveChanges();

                string path = Path.Combine(Server.MapPath("~/Members/"), Request.Cookies["ID"].Value);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (obj.ProfilePicture != null && obj.ProfilePicture.ContentLength > 0)
                {
                    string fileName = Path.GetFileNameWithoutExtension(obj.ProfilePicture.FileName);
                    string extension = Path.GetExtension(obj.ProfilePicture.FileName);
                    fileName = "DP_" + DateTime.Now.ToString("ddMMyyyy") + extension;
                    string finalpath = Path.Combine(path, fileName);
                    obj.ProfilePicture.SaveAs(finalpath);

                    uprof.ProfilePicture = Path.Combine(("~/Members/" + Request.Cookies["ID"].Value + "/"), fileName);
                    objNotesEntities.SaveChanges();
                }

                return RedirectToAction("Dashboard", "Front");
            }
            ModelState.Clear();
            return RedirectToAction("UserProfile", "Front");

        }






    }
}