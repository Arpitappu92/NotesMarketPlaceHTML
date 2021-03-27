using Notes_MarketPlace.Models;
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Net;
using PagedList;
using System.IO.Compression;
using PagedList.Mvc;
using System.Data.Entity;
using Notes_MarketPlace.AccRelTemplate;
using Notes_MarketPlace.Helpers;
  
namespace Notes_MarketPlace.Controllers
{

    [Authorize]
    [RoutePrefix("Front")]
    public class FrontController : Controller
    {
        private NotesEntities objNotesEntities = new NotesEntities();

        [Route("Dashboard")]
        // GET: Front
        public ActionResult Dashboard(string Sorting_Order_Progress, string Sorting_Order_Publish, string Search_Progress, string Search_Publish, int? noteProgressPage, int? notePublishedPage)
        {
            int id = Int16.Parse(Request.Cookies["ID"].Value);

            /*
                             ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "Name_Description" : "";
            */
            ViewBag.DateSortedProgress = string.IsNullOrEmpty(Sorting_Order_Progress) ? "CreatedDate_asc" : "";
            ViewBag.TitleSortedProgress = Sorting_Order_Progress == "Title" ? "Title_desc" : "Title";
            ViewBag.CategorySortedProgress = Sorting_Order_Progress == "Category" ? "Category_desc" : "Category";
            ViewBag.StatusSortedProgress = Sorting_Order_Progress == "Status" ? "Status_desc" : "Status";
            /* List<SellerNotes> sn = objNotesEntities.SellerNotes.ToList();
 */
            /*var note = objNotesEntities.SellerNotes.ToList().Where(x => x.SellerID == id && x.IsActive == true);
            ViewBag.NoteData = note;
*/
            List<SellerNotes> NoteTitle = objNotesEntities.SellerNotes.OrderByDescending(x => x.CreatedDate).Where(x => /*x.ActionedBy == id &&*/ x.IsActive == true && (x.NoteTitle.Contains(Search_Progress) || Search_Progress == null)).ToList();
            List<NoteCategories> CategoryName = objNotesEntities.NoteCategories.ToList();
            List<ReferenceData> StatusName = objNotesEntities.ReferenceData.Where(x => x.RefCategory == "Note Status" && x.Value != "Rejected" && x.Value != "Removed" && x.IsActive == true).ToList();

            /*  var note = from x in objNotesEntities.SellerNotes select x;*/
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
            var noteProgress = (from noti in NoteTitle
                                join catnam in CategoryName on noti.NoteCategory equals catnam.ID into table1
                                from catnam in table1.ToList()
                                join satnam in StatusName on noti.Status equals satnam.ID into table2
                                from satnam in table2.ToList()
                                where satnam.Value != "Published"
                                select new ProgressNote
                                {
                                    NoteData = noti,
                                    Category = catnam,
                                    status = satnam
                                }).AsQueryable();

            /*            var sn = objNotesEntities.SellerNotes.ToList();
            */
            switch (Sorting_Order_Progress)
            {
                case "CreatedDate_asc":
                    noteProgress = noteProgress.OrderBy(x => x.NoteData.CreatedDate);
                    break;
                case "Title_desc":
                    noteProgress = noteProgress.OrderByDescending(x => x.NoteData.NoteTitle);
                    break;
                case "Title":
                    noteProgress = noteProgress.OrderBy(x => x.NoteData.NoteTitle);
                    break;
                case "Category_desc":
                    noteProgress = noteProgress.OrderByDescending(x => x.Category.CategoryName);
                    break;
                case "Category":
                    noteProgress = noteProgress.OrderBy(x => x.Category.CategoryName);
                    break;
                case "Status_desc":
                    noteProgress = noteProgress.OrderByDescending(x => x.status.Value);
                    break;
                case "Status":
                    noteProgress = noteProgress.OrderBy(x => x.status.Value);
                    break;
                default:
                    noteProgress = noteProgress.OrderByDescending(x => x.NoteData.CreatedDate);
                    break;
            }

            ViewBag.noteProgress = noteProgress.ToList().ToPagedList(noteProgressPage ?? 1, 4);





            List<SellerNotes> NoteTitleForPublish = objNotesEntities.SellerNotes.OrderByDescending(x => x.CreatedDate).Where(x => /*x.ActionedBy == id &&*/ x.IsActive == true && (x.NoteTitle.Contains(Search_Publish) || Search_Publish == null)).ToList();
            List<NoteCategories> CategoryNameForPublish = objNotesEntities.NoteCategories.ToList();
            List<ReferenceData> StatusNameForPublish = objNotesEntities.ReferenceData.Where(x => x.RefCategory == "Note Status" && x.Value == "Published" && x.Value != "Removed" && x.IsActive == true).ToList();


            ViewBag.DateSortPublish = string.IsNullOrEmpty(Sorting_Order_Publish) ? "CreatedDate_asc" : "";
            ViewBag.TitleSortPublish = Sorting_Order_Publish == "Title" ? "Title_desc" : "Title";
            ViewBag.CategorySortPublish = Sorting_Order_Publish == "Category" ? "Category_desc" : "Category";
            ViewBag.SellTypeSortPublish = Sorting_Order_Publish == "Type" ? "Type_desc" : "Type";
            ViewBag.PriceSortPublish = Sorting_Order_Publish == "Price" ? "Price_desc" : "Price";


            var notePublish = (from noti in NoteTitleForPublish
                               join catnam in CategoryNameForPublish on noti.NoteCategory equals catnam.ID into table1
                               from catnam in table1.ToList()
                               join satnam in StatusNameForPublish on noti.Status equals satnam.ID into table2
                               from satnam in table2.ToList()
                               where satnam.Value == "Published"
                               select new PublishNote
                               {
                                   NoteData = noti,
                                   Category = catnam,
                                   status = satnam
                               }).AsQueryable();

            switch (Sorting_Order_Publish)
            {
                case "CreatedDate_asc":
                    notePublish = notePublish.OrderBy(x => x.NoteData.PublishedDate);
                    break;
                case "Title_desc":
                    notePublish = notePublish.OrderByDescending(x => x.NoteData.NoteTitle);
                    break;
                case "Title":
                    notePublish = notePublish.OrderBy(x => x.NoteData.NoteTitle);
                    break;
                case "Category_desc":
                    notePublish = notePublish.OrderByDescending(x => x.Category.CategoryName);
                    break;
                case "Category":
                    notePublish = notePublish.OrderBy(x => x.Category.CategoryName);
                    break;
                case "Type_desc":
                    notePublish = notePublish.OrderByDescending(x => x.NoteData.isPaid);
                    break;
                case "Type":
                    notePublish = notePublish.OrderBy(x => x.NoteData.isPaid);
                    break;
                case "Price_desc":
                    notePublish = notePublish.OrderByDescending(x => x.NoteData.SellPrice);
                    break;
                case "Price":
                    notePublish = notePublish.OrderBy(x => x.NoteData.SellPrice);
                    break;
                default:
                    notePublish = notePublish.OrderByDescending(x => x.NoteData.PublishedDate);
                    break;
            }

            ViewBag.notePublish = notePublish.ToList().ToPagedList(notePublishedPage ?? 1, 4);

            /*   {
             *   
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
                    Status = 6,
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







        [Route("EditNote/{id}")]
        [HttpGet]
        public ActionResult EditNote(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SellerNotes sellerNote = objNotesEntities.SellerNotes.Find(id);
            SellerNotesAttachements attachment = objNotesEntities.SellerNotesAttachements.Where(x => x.NoteID == sellerNote.ID).FirstOrDefault();

            SellerNotesModel sellerNote1 = new SellerNotesModel
            {
                ID = sellerNote.ID,
                NoteTitle = sellerNote.NoteTitle,
                NoteCategory = sellerNote.NoteCategory,
                NoteDescription = sellerNote.NoteDescription,
                isPaid = sellerNote.isPaid,
                NoteType = sellerNote.NoteType,
                NumberOfPages = sellerNote.NumberOfPages,
                UniversityInformation = sellerNote.UniversityInformation,
                Country = sellerNote.Country,
                Course = sellerNote.Course,
                CourseCode = sellerNote.CourseCode,
                ProfessorName = sellerNote.ProfessorName,
                SellPrice = sellerNote.SellPrice,


            };



            if (sellerNote == null)
            {
                return HttpNotFound();
            }
            ViewBag.NotePriview = sellerNote.PreviewUpload;
            ViewBag.AttechmentPath = attachment.FilePath;
            ViewBag.DP = sellerNote.DisplayPicture;
            /*ViewBag.Country = new SelectList(objNotesEntities.Country, "CoutryID", "Name", sellerNote.Country);
            ViewBag.Category = new SelectList(objNotesEntities.NoteCategories, "CategoryID", "Name", sellerNote.NoteCategory);
            ViewBag.NoteType = new SelectList(objNotesEntities.NoteTypes, "TypeID", "Name", sellerNote.NoteType);*/
            ViewBag.Country = objNotesEntities.Country.Where(x => x.IsActive == true);
            ViewBag.NoteType = objNotesEntities.NoteTypes.Where(x => x.IsActive == true);
            ViewBag.NoteCategory = objNotesEntities.NoteCategories.Where(x => x.IsActive == true);
            return View(sellerNote1);

        }

        [Route("EditNote/{id}")]
        [HttpPost]
        public ActionResult EditNote(int? id, SellerNotesModel objAddNote, string submit)
        {
            if (ModelState.IsValid)
            {

                //Check UploadNote Is Selected Or Not
/*                if (objAddNote.UploadNote == null)
                {
                    TempData["notice"] = "Select File to upload";
                    ViewBag.Country = new SelectList(objNotesEntities.Country.Where(x => x.IsActive == true), "CoutryID", "Name", objAddNote.Country);
                    ViewBag.Category = new SelectList(objNotesEntities.NoteCategories.Where(x => x.IsActive == true), "CategoryID", "Name", objAddNote.NoteCategory);
                    ViewBag.NoteType = new SelectList(objNotesEntities.NoteTypes.Where(x => x.IsActive == true), "TypeID ", "Name", objAddNote.NoteType);
                    return View(objAddNote);
                }

                if (objAddNote.isPaid == true && objAddNote.SellPrice == null)
                {
                    TempData["noticePrice"] = "Enter The Price";
                    ViewBag.Country = new SelectList(objNotesEntities.Country.Where(x => x.IsActive == true), "CoutryID", "Name", objAddNote.Country);
                    ViewBag.Category = new SelectList(objNotesEntities.NoteCategories.Where(x => x.IsActive == true), "CategoryID", "Name", objAddNote.NoteCategory);
                    ViewBag.NoteType = new SelectList(objNotesEntities.NoteTypes.Where(x => x.IsActive == true), "TypeID ", "Name", objAddNote.NoteType);
                    *//* ViewBag.NotesCategory = objNotesEntities.NoteCategories.Where(x => x.IsActive == true);
                     ViewBag.NotesType = objNotesEntities.NoteTypes.Where(x => x.IsActive == true);
                     ViewBag.Country = objNotesEntities.Country.Where(x => x.IsActive == true);*//*
                    return View(objAddNote);
                }

                if (objAddNote.isPaid == true && objAddNote.PreviewUpload == null)
                {
                    TempData["noticePreview"] = "Note Preview Is Required For Paid Notes";
                    ViewBag.Country = new SelectList(objNotesEntities.Country.Where(x => x.IsActive == true), "CoutryID", "Name", objAddNote.Country);
                    ViewBag.Category = new SelectList(objNotesEntities.NoteCategories.Where(x => x.IsActive == true), "CategoryID", "Name", objAddNote.NoteCategory);
                    ViewBag.NoteType = new SelectList(objNotesEntities.NoteTypes.Where(x => x.IsActive == true), "TypeID ", "Name", objAddNote.NoteType);
                    return View(objAddNote);
                }
*/
                /*var EmailID = Request.Cookies["EmailID"].Value.ToString();*/
                var EmailID = User.Identity.Name.ToString();
                Users userObj = objNotesEntities.Users.Where(x => x.EmailId == EmailID).FirstOrDefault();
                string path = Path.Combine(Server.MapPath("~/Members"), userObj.ID.ToString());

                // Check for Directory, If not exist, then create it  
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var notevalue = "";
                if (submit == "Publish")
                {
                    notevalue = "Submitted For Review";

                    PublishedNotesEmailtemp.PublishedNoteNotify(userObj, objAddNote.NoteTitle);
                }
                else if (submit == "Save")
                {
                    notevalue = "Draft";
                }

                ReferenceData referenceData = objNotesEntities.ReferenceData.Where(x => x.RefCategory == "Note Status" && x.Value == "Draft" && x.IsActive == true).FirstOrDefault();

                SellerNotes objSellerNote = objNotesEntities.SellerNotes.Where(x => x.ID == objAddNote.ID).FirstOrDefault();

                objSellerNote.Status = referenceData.ID;
                objSellerNote.NoteTitle = objAddNote.NoteTitle;
                objSellerNote.NoteCategory = objAddNote.NoteCategory;
                objSellerNote.NoteDescription = objAddNote.NoteDescription;
                objSellerNote.isPaid = objAddNote.isPaid;
                objSellerNote.NoteType = objAddNote.NoteType;
                objSellerNote.NumberOfPages = objAddNote.NumberOfPages;
                objSellerNote.UniversityInformation = objAddNote.UniversityInformation;
                objSellerNote.Country = objAddNote.Country;
                objSellerNote.Course = objAddNote.Course;
                objSellerNote.CourseCode = objAddNote.CourseCode;
                objSellerNote.ProfessorName = objAddNote.ProfessorName;
                objSellerNote.SellPrice = objAddNote.SellPrice;
                objSellerNote.CreatedBy = userObj.ID;
                objSellerNote.IsActive = true;
                objSellerNote.ModifiedDate = DateTime.Now;
                objSellerNote.ModifiedBy = userObj.ID;
                objNotesEntities.Entry(objSellerNote).State = EntityState.Modified;
                objNotesEntities.SaveChanges();

                var noteID = objSellerNote.ID;
                SellerNotes sellerNote = objNotesEntities.SellerNotes.Where(x => x.NoteTitle == objAddNote.NoteTitle && x.CreatedBy == userObj.ID).FirstOrDefault();

                string storepath = Path.Combine(Server.MapPath("~/Members/" + userObj.ID), noteID.ToString());

                System.IO.DirectoryInfo di = new DirectoryInfo(storepath);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }


                // Check for Directory, If not exist, then create it  
                if (!Directory.Exists(storepath))
                {
                    Directory.CreateDirectory(storepath);
                }

                if (objAddNote.DisplayPicture != null && objAddNote.DisplayPicture.ContentLength > 0)
                {
                    string fileName = Path.GetFileNameWithoutExtension(objAddNote.DisplayPicture.FileName);
                    string extension = Path.GetExtension(objAddNote.DisplayPicture.FileName);
                    fileName = "DP_" + DateTime.Now.ToString("ddMMyyyy") + extension;
                    string finalpath = Path.Combine(storepath, fileName);
                    objAddNote.DisplayPicture.SaveAs(finalpath);

                    objSellerNote.DisplayPicture = Path.Combine(("/Members/" + userObj.ID + "/" + noteID + "/"), fileName);
                    objNotesEntities.SaveChanges();
                }
                else
                {
                    objSellerNote.DisplayPicture = Path.Combine("/SystemConfiguration/DefaultImage/", "DefaultNoteImage.jpg");

                    objNotesEntities.SaveChanges();

                }


                if (objAddNote.PreviewUpload != null && objAddNote.PreviewUpload.ContentLength > 0)
                {
                    string fileName = Path.GetFileNameWithoutExtension(objAddNote.PreviewUpload.FileName);
                    string extension = Path.GetExtension(objAddNote.PreviewUpload.FileName);
                    fileName = "Preview_" + DateTime.Now.ToString("ddMMyyyy") + extension;
                    string finalpath = Path.Combine(storepath, fileName);
                    objAddNote.PreviewUpload.SaveAs(finalpath);

                    objSellerNote.PreviewUpload = Path.Combine(("/Members/" + userObj.ID + "/" + noteID + "/"), fileName);
                    objNotesEntities.SaveChanges();
                }

                string attachementsstorepath = Path.Combine(storepath, "Attachements");

                // Check for Directory, If not exist, then create it  
                if (!Directory.Exists(attachementsstorepath))
                {
                    Directory.CreateDirectory(attachementsstorepath);
                }

                SellerNotesAttachements sellerNotesAttachement = objNotesEntities.SellerNotesAttachements.Where(x => x.NoteID == noteID).FirstOrDefault();
                sellerNotesAttachement.ModifiedDate = DateTime.Now;
                sellerNotesAttachement.ModifiedBy = userObj.ID;

                int Count = 1;
                var FilePath = "";
                var FileName = "";
                if(objAddNote.UploadNote != null && objAddNote.UploadNote.ContentLength > 0)
                {
                    string fileName = Path.GetFileNameWithoutExtension(objAddNote.UploadNote.FileName);
                    string extension = Path.GetExtension(objAddNote.UploadNote.FileName);
                    fileName = "Attachement" + Count + "_" + DateTime.Now.ToString("ddMMyyyy") + extension;
                    string finalpath = Path.Combine(attachementsstorepath, fileName);
                    objAddNote.UploadNote.SaveAs(finalpath);

                    FileName += fileName + ";";
                    FilePath += Path.Combine(("/Members/" + userObj.ID + "/" + noteID + "/Attachements/"), fileName) + ";";

                    Count++;
                }

                sellerNotesAttachement.FilePath = FilePath;
                sellerNotesAttachement.FileName = FileName;
                objNotesEntities.Entry(sellerNotesAttachement).State = EntityState.Modified;
                objNotesEntities.SaveChanges();

                return RedirectToAction("Dashboard", "Front");
            }

            /*           ViewBag.Country = new SelectList(objNotesEntities.Country.Where(x => x.IsActive == true), "CoutryID", "Name", objAddNote.Country);
                       ViewBag.Category = new SelectList(objNotesEntities.NoteCategories.Where(x => x.IsActive == true), "CategoryID", "Name", objAddNote.NoteCategory);
                       ViewBag.NoteType = new SelectList(objNotesEntities.NoteTypes.Where(x => x.IsActive == true), "TypeID ", "Name", objAddNote.NoteType);*/

            ViewBag.Country = objNotesEntities.Country.Where(x => x.IsActive == true);
            ViewBag.NoteType = objNotesEntities.NoteTypes.Where(x => x.IsActive == true);
            ViewBag.NoteCategory = objNotesEntities.NoteCategories.Where(x => x.IsActive == true);
            return View(objAddNote);
        }


        [Route("DeleteNote/{id}")]
        [HttpGet]
        public ActionResult DeleteNote(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SellerNotes sellerNote = objNotesEntities.SellerNotes.Find(id);
            if (sellerNote == null)
            {
                return HttpNotFound();
            }

            string storepath = Path.Combine(Server.MapPath("~/Members/" + sellerNote.SellerID), sellerNote.ID.ToString());
            System.IO.DirectoryInfo di = new DirectoryInfo(storepath);
            // Check for Directory, If not exist, then create it  
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            objNotesEntities.SellerNotes.Remove(sellerNote);
            objNotesEntities.SaveChanges();


            return RedirectToAction("Dashboard", "Front");

        }



        [AllowAnonymous]
        [Route("NoteDetails/{id}")]
        public ActionResult NoteDetails(int? id)
        {



            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SellerNotes sellernote = objNotesEntities.SellerNotes.Find(id);
            if (sellernote == null)
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


        [Route("DownloadNote/{id}")]
        public ActionResult DownloadNote(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SellerNotes sellerNote = objNotesEntities.SellerNotes.Find(id);


            if (sellerNote == null)
            {
                return HttpNotFound();
            }
            var EmailID = User.Identity.Name.ToString();
            Users userObj = objNotesEntities.Users.Where(x => x.EmailId == EmailID).FirstOrDefault();

            SellerNotesAttachements attechment = objNotesEntities.SellerNotesAttachements.Where(x => x.NoteID == sellerNote.ID).FirstOrDefault();
            NoteCategories category = objNotesEntities.NoteCategories.Where(x => x.ID == sellerNote.NoteCategory).FirstOrDefault(); ;
            if (sellerNote.isPaid == false)
            {
                Downloads download = new Downloads
                {
                    NoteID = sellerNote.ID,
                    Seller = sellerNote.SellerID,
                    Downloader = userObj.ID,
                    IsSellerHasAllowedDownload = true,
                    AttachmentPath = attechment.FilePath,
                    IsAttachedDownloaded = true,
                    AttachmentDownloadedDate = DateTime.Now,
                    IsPaid = sellerNote.isPaid,
                    PurchasedPrice = sellerNote.SellPrice,
                    NoteTitle = sellerNote.NoteTitle,
                    NoteCategory = category.CategoryName,
                    CreatedDate = DateTime.Now,
                    CreatedBy = userObj.ID,
                    ModifiedDate = DateTime.Now,
                    ModifiedBy = userObj.ID,
                 
                };

                objNotesEntities.Downloads.Add(download);
                objNotesEntities.SaveChanges();

                var allFilesPath = attechment.FilePath.Split(';');
                var allFileName = attechment.FileName.Split(';');

                using (var memoryStream = new MemoryStream())
                {
                    using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var FilePath in allFilesPath)
                        {
                            string FullPath = Path.Combine(Server.MapPath("" + FilePath));
                            string FileName = Path.GetFileName(FullPath);
                            if (FileName == "")
                            {
                                continue;
                            }
                            else
                            {
                                ziparchive.CreateEntryFromFile(FullPath,FileName);
                                
                            }
                        }
                    }
                    return File(memoryStream.ToArray(), "application/zip", "Attachments.zip");
                }

                return RedirectToAction("Dashboard", "Front");
            }
            else
            {

                Downloads download = new Downloads
                {
                    NoteID = sellerNote.ID,
                    Seller = sellerNote.SellerID,
                    Downloader = userObj.ID,
                    IsSellerHasAllowedDownload = false,
                    AttachmentPath = null,
                    IsAttachedDownloaded = false,
                    AttachmentDownloadedDate = DateTime.Now,
                    IsPaid = sellerNote.isPaid,
                    PurchasedPrice = sellerNote.SellPrice,
                    NoteTitle = sellerNote.NoteTitle,
                    NoteCategory = category.CategoryName,
                    CreatedDate = DateTime.Now,
                    CreatedBy = userObj.ID,
                    ModifiedDate = DateTime.Now,
                    ModifiedBy = userObj.ID,
            
                };

                objNotesEntities.Downloads.Add(download);
                objNotesEntities.SaveChanges();

                Users sellerRecord = objNotesEntities.Users.Find(sellerNote.SellerID);
                /*var sellerEmail = sellerRecord.EmailID;*/

                BuyerReqEmail.BuyerNotifyEmail(userObj, sellerRecord);

            }



            return RedirectToAction("Dashboard", "Front");
        }






        public ActionResult GetPP()
        {
            int id = Int16.Parse(Request.Cookies["ID"].Value);
            UserProfile xy = objNotesEntities.UserProfile.Where(x => x.UserID == id).FirstOrDefault();
            var pp = xy.ProfilePicture;
            return Content(pp); // Of whatever you need to return.
        }










        [Route("BuyerRequest")]
        public ActionResult BuyerRequest(string SearchNoteTitle, string SortOrder, int? page)
        {

            var EmailID = User.Identity.Name.ToString();
            Users userObj = objNotesEntities.Users.Where(x => x.EmailId == EmailID).FirstOrDefault();

            ViewBag.DateSortParm = String.IsNullOrEmpty(SortOrder) ? "Date_asc" : "";
            ViewBag.TitleSortParm = SortOrder == "Title" ? "Title_desc" : "Title";
            ViewBag.CategorySortParm = SortOrder == "Category" ? "Category_desc" : "Category";
            ViewBag.PriceSortParm = SortOrder == "Price" ? "Price_desc" : "Price";

            List<Downloads> Download = objNotesEntities.Downloads.Where(x => x.Seller == userObj.ID && x.IsSellerHasAllowedDownload == false && x.IsPaid == true && (x.NoteTitle.Contains(SearchNoteTitle) || x.NoteCategory.Contains(SearchNoteTitle) || x.PurchasedPrice.ToString().StartsWith(SearchNoteTitle) || SearchNoteTitle == null)).ToList();
            List<Users> user = objNotesEntities.Users.ToList();
            List<UserProfile> userprofile = objNotesEntities.UserProfile.ToList();

            var BuyerRequestsNote = (from nt in Download
                                     join cn in user on nt.Downloader equals cn.ID into table1
                                     from cn in table1.ToList()
                                     join up in userprofile on nt.Downloader equals up.UserID into table2
                                     from up in table2.ToList()
                                     select new BuyerRequest
                                     {
                                         DownloadNote = nt,
                                         BuyerDetail = cn,
                                         UserProfileNote = up
                                     }).AsQueryable();



            switch (SortOrder)
            {
                case "Date_asc":
                    BuyerRequestsNote = BuyerRequestsNote.OrderBy(x => x.DownloadNote.CreatedDate);
                    break;
                case "Title_desc":
                    BuyerRequestsNote = BuyerRequestsNote.OrderByDescending(x => x.DownloadNote.NoteTitle);
                    break;
                case "Title":
                    BuyerRequestsNote = BuyerRequestsNote.OrderBy(x => x.DownloadNote.NoteTitle);
                    break;
                case "Category_desc":
                    BuyerRequestsNote = BuyerRequestsNote.OrderByDescending(x => x.DownloadNote.NoteCategory);
                    break;
                case "Category":
                    BuyerRequestsNote = BuyerRequestsNote.OrderBy(x => x.DownloadNote.NoteCategory);
                    break;
                case "Price_desc":
                    BuyerRequestsNote = BuyerRequestsNote.OrderByDescending(x => x.DownloadNote.PurchasedPrice);
                    break;
                case "Price":
                    BuyerRequestsNote = BuyerRequestsNote.OrderBy(x => x.DownloadNote.PurchasedPrice);
                    break;
                default:
                    BuyerRequestsNote = BuyerRequestsNote.OrderByDescending(x => x.DownloadNote.CreatedDate);
                    break;
            }

            return View(BuyerRequestsNote.ToList().ToPagedList(page ?? 1, 5));
        }


        [Route("MyDownloads")]
        public ActionResult MyDownloads(int? page, string SearchNoteTitle, string SortOrder)
        {
            var EmailID = User.Identity.Name.ToString();
            Users userObj = objNotesEntities.Users.Where(x => x.EmailId == EmailID).FirstOrDefault();

            ViewBag.DateSortParm = String.IsNullOrEmpty(SortOrder) ? "Date_asc" : "";
            ViewBag.TitleSortParm = SortOrder == "Title" ? "Title_desc" : "Title";
            ViewBag.CategorySortParm = SortOrder == "Category" ? "Category_desc" : "Category";
            ViewBag.SellTypeSortParm = SortOrder == "SellType" ? "SellType_desc" : "SellType";
            ViewBag.PriceSortParm = SortOrder == "Price" ? "Price_desc" : "Price";

            List<Downloads> Download = objNotesEntities.Downloads.Where(x => x.Downloader == userObj.ID && x.IsSellerHasAllowedDownload == true && x.AttachmentPath != null && (x.NoteTitle.Contains(SearchNoteTitle) || x.NoteCategory.Contains(SearchNoteTitle) || x.PurchasedPrice.ToString().StartsWith(SearchNoteTitle) || SearchNoteTitle == null)).ToList();
            List<Users> user = objNotesEntities.Users.ToList();
            /*List<UserProfile> userprofile = objNotesEntities.UserProfile.ToList();*/
            /* (x.NoteTitle.Contains(SearchNoteTitle) || x.NoteCategory.Contains(SearchNoteTitle) || x.PurchasedPrice.ToString().StartsWith(SearchNoteTitle) || string.IsNullOrEmpty(SearchNoteTitle))*/

            var MyDownloadsNote = (from nt in Download
                                   join cn in user on nt.Downloader equals cn.ID into table1
                                   from cn in table1.ToList()

                                   select new MyDownloadNote
                                   {
                                       DownloadNote = nt,
                                       BuyerDetail = cn,

                                   }).AsQueryable();

            switch (SortOrder)
            {
                case "Date_asc":
                    MyDownloadsNote = MyDownloadsNote.OrderBy(x => x.DownloadNote.CreatedDate);
                    break;
                case "Title_desc":
                    MyDownloadsNote = MyDownloadsNote.OrderByDescending(x => x.DownloadNote.NoteTitle);
                    break;
                case "Title":
                    MyDownloadsNote = MyDownloadsNote.OrderBy(x => x.DownloadNote.NoteTitle);
                    break;
                case "Category_desc":
                    MyDownloadsNote = MyDownloadsNote.OrderByDescending(x => x.DownloadNote.NoteCategory);
                    break;
                case "Category":
                    MyDownloadsNote = MyDownloadsNote.OrderBy(x => x.DownloadNote.NoteCategory);
                    break;
                case "SellType_desc":
                    MyDownloadsNote = MyDownloadsNote.OrderByDescending(x => x.DownloadNote.IsPaid);
                    break;
                case "SellType":
                    MyDownloadsNote = MyDownloadsNote.OrderBy(x => x.DownloadNote.IsPaid);
                    break;
                case "Price_desc":
                    MyDownloadsNote = MyDownloadsNote.OrderByDescending(x => x.DownloadNote.PurchasedPrice);
                    break;
                case "Price":
                    MyDownloadsNote = MyDownloadsNote.OrderBy(x => x.DownloadNote.PurchasedPrice);
                    break;
                default:
                    MyDownloadsNote = MyDownloadsNote.OrderByDescending(x => x.DownloadNote.CreatedDate);
                    break;
            }

            return View(MyDownloadsNote.ToList().ToPagedList(page ?? 1, 5));
        }

        [Route("MySoldNotes")]
        public ActionResult MySoldNotes(int? page, string SearchNoteTitle, string SortOrder)
        {
            var EmailID = User.Identity.Name.ToString();
            Users userObj = objNotesEntities.Users.Where(x => x.EmailId == EmailID).FirstOrDefault();

            ViewBag.DateSortParm = String.IsNullOrEmpty(SortOrder) ? "Date_asc" : "";
            ViewBag.TitleSortParm = SortOrder == "Title" ? "Title_desc" : "Title";
            ViewBag.CategorySortParm = SortOrder == "Category" ? "Category_desc" : "Category";
            ViewBag.SellTypeSortParm = SortOrder == "SellType" ? "SellType_desc" : "SellType";
            ViewBag.PriceSortParm = SortOrder == "Price" ? "Price_desc" : "Price";

            List<Downloads> Download = objNotesEntities.Downloads.Where(x => x.Seller == userObj.ID && x.IsSellerHasAllowedDownload == true && x.AttachmentPath != null && (x.NoteTitle.Contains(SearchNoteTitle) || x.NoteCategory.Contains(SearchNoteTitle) || x.PurchasedPrice.ToString().StartsWith(SearchNoteTitle) || SearchNoteTitle == null)).ToList();
            List<Users> user = objNotesEntities.Users.ToList();
            /*List<UserProfile> userprofile = objNotesEntities.UserProfile.ToList();*/
            /* (x.NoteTitle.Contains(SearchNoteTitle) || x.NoteCategory.Contains(SearchNoteTitle) || x.PurchasedPrice.ToString().StartsWith(SearchNoteTitle) || string.IsNullOrEmpty(SearchNoteTitle))*/

            var MySoldsNote = (from nt in Download
                               join cn in user on nt.Downloader equals cn.ID into table1
                               from cn in table1.ToList()
                               select new MySoldNote
                               {
                                   DownloadNote = nt,
                                   BuyerDetail = cn,

                               }).AsQueryable();


            switch (SortOrder)
            {
                case "Date_asc":
                    MySoldsNote = MySoldsNote.OrderBy(x => x.DownloadNote.CreatedDate);
                    break;
                case "Title_desc":
                    MySoldsNote = MySoldsNote.OrderByDescending(x => x.DownloadNote.NoteTitle);
                    break;
                case "Title":
                    MySoldsNote = MySoldsNote.OrderBy(x => x.DownloadNote.NoteTitle);
                    break;
                case "Category_desc":
                    MySoldsNote = MySoldsNote.OrderByDescending(x => x.DownloadNote.NoteCategory);
                    break;
                case "Category":
                    MySoldsNote = MySoldsNote.OrderBy(x => x.DownloadNote.NoteCategory);
                    break;
                case "SellType_desc":
                    MySoldsNote = MySoldsNote.OrderByDescending(x => x.DownloadNote.IsPaid);
                    break;
                case "SellType":
                    MySoldsNote = MySoldsNote.OrderBy(x => x.DownloadNote.IsPaid);
                    break;
                case "Price_desc":
                    MySoldsNote = MySoldsNote.OrderByDescending(x => x.DownloadNote.PurchasedPrice);
                    break;
                case "Price":
                    MySoldsNote = MySoldsNote.OrderBy(x => x.DownloadNote.PurchasedPrice);
                    break;
                default:
                    MySoldsNote = MySoldsNote.OrderByDescending(x => x.DownloadNote.CreatedDate);
                    break;
            }

            return View(MySoldsNote.ToList().ToPagedList(page ?? 1, 5));
        }

        [Route("MyRejectedNote")]
        public ActionResult MyRejectedNote(int? page, string SearchNoteTitle, string SortOrderProgress)
        {
            var EmailID = User.Identity.Name.ToString();
            Users userObj = objNotesEntities.Users.Where(x => x.EmailId == EmailID).FirstOrDefault();

            ViewBag.DateSortParm = String.IsNullOrEmpty(SortOrderProgress) ? "Date_asc" : "";
            ViewBag.TitleSortParm = SortOrderProgress == "Title" ? "Title_desc" : "Title";
            ViewBag.CategorySortParm = SortOrderProgress == "Category" ? "Category_desc" : "Category";


            List<SellerNotes> NoteTitle = objNotesEntities.SellerNotes.Where(x => x.SellerID == userObj.ID && x.IsActive == true && (x.NoteTitle.Contains(SearchNoteTitle) || x.NoteCategories.CategoryName.Contains(SearchNoteTitle) || x.AdminRemarks.Contains(SearchNoteTitle) || SearchNoteTitle == null)).ToList();
            List<NoteCategories> CategoryName = objNotesEntities.NoteCategories.ToList();
            List<ReferenceData> StatusName = objNotesEntities.ReferenceData.Where(x => x.RefCategory == "Note Status" && x.Value != "Removed" && x.IsActive == true).ToList();

            var MyRejectedNote = (from nt in NoteTitle
                                  join cn in CategoryName on nt.NoteCategory equals cn.ID into table1
                                  from cn in table1.ToList()
                                  join sn in StatusName on nt.Status equals sn.ID into table2
                                  from sn in table2.ToList()
                                  where sn.Value == "Rejected"
                                  select new RejectedNote
                                  {
                                      NoteDetails = nt,
                                      Category = cn,
                                      status = sn
                                  }).AsQueryable();

            switch (SortOrderProgress)
            {
                case "Date_asc":
                    MyRejectedNote = MyRejectedNote.OrderBy(x => x.NoteDetails.ModifiedDate);
                    break;
                case "Title_desc":
                    MyRejectedNote = MyRejectedNote.OrderByDescending(x => x.NoteDetails.NoteTitle);
                    break;
                case "Title":
                    MyRejectedNote = MyRejectedNote.OrderBy(x => x.NoteDetails.NoteTitle);
                    break;
                case "Category_desc":
                    MyRejectedNote = MyRejectedNote.OrderByDescending(x => x.Category.CategoryName);
                    break;
                case "Category":
                    MyRejectedNote = MyRejectedNote.OrderBy(x => x.Category.CategoryName);
                    break;
                default:
                    MyRejectedNote = MyRejectedNote.OrderByDescending(x => x.NoteDetails.ModifiedDate);
                    break;
            }
            ViewBag.MyRejectedNote = MyRejectedNote.ToList().ToPagedList(page ?? 1, 4);

            return View();
        }


        [Route("Clone/{id}")]
        public ActionResult Clone(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var Emailid = User.Identity.Name.ToString();
            Users user = objNotesEntities.Users.Where(x => x.EmailId == Emailid).FirstOrDefault();

            SellerNotes note = objNotesEntities.SellerNotes.Find(id);

            if (note == null)
            {
                return RedirectToAction("Error", "HomePage");
            }

            note.Status = objNotesEntities.ReferenceData.Where(x => x.RefCategory == "Note Status" && x.Value == "Draft").Select(x => x.ID).FirstOrDefault();
            note.AdminRemarks = null;
            note.ModifiedBy = user.ID;
            note.ModifiedDate = DateTime.Now;

            objNotesEntities.Entry(note).State = EntityState.Modified;
            objNotesEntities.SaveChanges();

            TempData["RejectedNote"] = user.FirstName + " " + user.LastName;
            TempData["Message"] = "! Your Clone Request has been Successfully !";

            return RedirectToAction("MyRejectedNote", "Front");

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



        [Route("ChangePassword")]
        [HttpGet]
        public ActionResult ChangePassword()
        {
            ViewBag.Title = "NotesMarketPlace";
            return View();
        }

        [Route("ChangePassword")]
        [HttpPost]
        public ActionResult ChangePassword(ChangePassword changePwd)
        {
            if (ModelState.IsValid)
            {


                if (User.Identity.IsAuthenticated)
                {
                    var Emailid = User.Identity.Name.ToString();

                    Users user = objNotesEntities.Users.Where(x => x.EmailId == Emailid).FirstOrDefault();

                    var pwd = EncryptPasswords.EncryptPasswordMd5(changePwd.OldPassword);

                    if (user.Password == pwd)
                    {
                        user.Password = EncryptPasswords.EncryptPasswordMd5(changePwd.NewPassword);
                        user.ModifiedDate = DateTime.Now;
                        user.ModifiedBy = user.ID;

                        objNotesEntities.Entry(user).State = EntityState.Modified;
                        objNotesEntities.SaveChanges();

                    }
                    else
                    {
                        TempData["IncorrectPwd"] = "Old Password Is Incorrect !";
                        return View();
                    }


                }
                else
                {
                    return View();
                }


            }
            return View();
        }







































        [Route("UpdateProfile")]
        [HttpGet]
        public ActionResult UpdateProfile()
        {
            ViewBag.Title = "Update Profile";
            var Emailid = User.Identity.Name.ToString();
            Users userObj = objNotesEntities.Users.Where(x => x.EmailId == Emailid).FirstOrDefault();

            if (User.Identity.IsAuthenticated)
            {
                UserProfile Profilepic = objNotesEntities.UserProfile.Where(x => x.UserID == userObj.ID).FirstOrDefault();
                if (Profilepic != null)
                {
                    TempData["ProfilePicture"] = Profilepic.ProfilePicture;
                }
                else
                {
                    TempData["ProfilePicture"] = Path.Combine("/SystemConfiguration/DefaultImage/", "DefaultUserImage.jpg");
                }
            }
            var v = objNotesEntities.Users.Where(x => x.EmailId == Emailid).FirstOrDefault();

            if (v != null)
            {
                UpdateFrontUserProfile profile = new UpdateFrontUserProfile
                {
                    FirstName = v.FirstName,
                    LastName = v.LastName,
                    EmailID = v.EmailId,
                    ID = v.ID
                };

                UserProfile ud = objNotesEntities.UserProfile.Where(x => x.UserID == v.ID).FirstOrDefault();

                if (ud != null)
                {
                    profile.DateOfBirth = ud.DateOfBirth;
                    profile.Gender = ud.Gender;
                    profile.PhoneCountryCode = ud.PhoneCountryCode;
                    profile.PhoneNumber = ud.PhoneNumber;
                    profile.AddressLine1 = ud.AddressLine1;
                    profile.AddressLine2 = ud.AddressLine2;
                    profile.City = ud.City;
                    profile.State = ud.State;
                    profile.ZipCode = ud.ZipCode;
                    profile.Country = ud.Country;
                    profile.University = ud.University;
                    profile.College = ud.College;
                    /*profile.ProfilePicture = ud.ProfilePicture;*/
                    ViewBag.ProfilePicture = ud.ProfilePicture;

                    /*
                                        ViewBag.Country = objNotesEntities.Country.Where(x => x.IsActive == true);
                                        ViewBag.NoteType = objNotesEntities.NoteTypes.Where(x => x.IsActive == true);
                                        ViewBag.NoteCategory = objNotesEntities.NoteCategories.Where(x => x.IsActive == true);

                    */
                    /*ViewBag.ProfilePicture = Path.Combine("/SystemConfiguration/DefaultImage/", "DefaultUserImage.jpg");*/
                    /*                    ViewBag.Country = new SelectList(objNotesEntities.Country.Distinct().Where(x => x.IsActive == true), "Name", "Name", profile.Country);
                                        ViewBag.PhoneCountryCode = new SelectList(objNotesEntities.Country.Distinct().Where(x => x.IsActive == true), "CountryCode", "CountryCode", profile.PhoneCountryCode);
                                        ViewBag.PhoneCountryCode = new SelectList(objNotesEntities.Country.Distinct().Where(x => x.IsActive == true), "CountryCode", "CountryCode", profile.PhoneCountryCode);
                     */

/*                    ViewBag.Country = objNotesEntities.Country.Where(x => x.IsActive == true);
*/                    ViewBag.Country = new SelectList(objNotesEntities.Country.Distinct().Where(x => x.IsActive == true), "ID", "CountryName", profile.Country);
                    ViewBag.PhoneCountryCode = new SelectList(objNotesEntities.Country.Distinct().Where(x => x.IsActive == true), "CountryCode", "CountryCode", profile.PhoneCountryCode);
                    ViewBag.Gender = new SelectList(objNotesEntities.ReferenceData.Where(x => x.RefCategory == "Gender" && x.IsActive == true), "ID", "DataValue").ToList();

                    return View(profile);
                }
                else
                {
                    /*ViewBag.ProfilePicture = Path.Combine("/SystemConfiguration/DefaultImage/", "DefaultUserImage.jpg");*/
                    ViewBag.Country = new SelectList(objNotesEntities.Country.Distinct().Where(x => x.IsActive == true), "Name", "Name");
                    ViewBag.PhoneCountryCode = new SelectList(objNotesEntities.Country.Distinct().Where(x => x.IsActive == true), "CountryCode", "CountryCode");
                    ViewBag.Gender = new SelectList(objNotesEntities.ReferenceData.Where(x => x.RefCategory == "Gender" && x.IsActive == true), "ID", "Value").ToList();
                    return View(profile);

                }

            }
            return View();

        }

        [Route("UpdateProfile")]
        [HttpPost]
        public ActionResult UpdateProfile(UpdateFrontUserProfile userdetails)
        {
            if (ModelState.IsValid)
            {
                var Emailid = User.Identity.Name.ToString();

                Users user = objNotesEntities.Users.Where(x => x.EmailId == Emailid).FirstOrDefault();

                user.FirstName = userdetails.FirstName;
                user.LastName = userdetails.LastName;


                var id = objNotesEntities.UserProfile.Where(x => x.UserID == userdetails.UserID).FirstOrDefault();

                /* var ud = objNotesEntities.UserProfile.Where(x => x.UserID == user.UserID).FirstOrDefault();*/
                if (id != null)
                {
                    UserProfile updatedetail = objNotesEntities.UserProfile.Where(x => x.UserID == userdetails.UserID).FirstOrDefault();
                    /*Convert.ToDateTime(userdetails.DateOfBirth).ToString("yyyy-MM-dd")*/
                    updatedetail.DateOfBirth = userdetails.DateOfBirth;
                    updatedetail.Gender = userdetails.Gender;
                    updatedetail.PhoneCountryCode = userdetails.PhoneCountryCode;
                    updatedetail.PhoneNumber = userdetails.PhoneNumber;
                    updatedetail.AddressLine1 = userdetails.AddressLine1;
                    updatedetail.AddressLine2 = userdetails.AddressLine2;
                    updatedetail.City = userdetails.City;
                    updatedetail.State = userdetails.State;
                    updatedetail.ZipCode = userdetails.ZipCode;
                    updatedetail.Country = userdetails.Country;
                    updatedetail.University = userdetails.University;
                    updatedetail.College = userdetails.College;
                    updatedetail.ModifiedBy = userdetails.UserID;
                    updatedetail.ModifiedDate = DateTime.Now;
                  
                    string storepath = Path.Combine(Server.MapPath("~/Members/"), user.ID.ToString());
                    System.IO.DirectoryInfo di = new DirectoryInfo(storepath);

                    /* FileInfo file = new FileInfo(updatedetail.ProfilePicture);
                     if (file.Exists)//check file exsit or not  
                     {
                         file.Delete();

                     }*/
                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                    /*foreach (DirectoryInfo dir in di.GetDirectories())
                    {
                        dir.Delete(true);
                    }*/

                    // Check for Directory, If not exist, then create it  
                    if (!Directory.Exists(storepath))
                    {
                        Directory.CreateDirectory(storepath);
                    }

                    if (userdetails.ProfilePicture != null && userdetails.ProfilePicture.ContentLength > 0)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(userdetails.ProfilePicture.FileName);
                        string extension = Path.GetExtension(userdetails.ProfilePicture.FileName);
                        fileName = "DP_" + DateTime.Now.ToString("ddMMyyyy") + extension;
                        string finalpath = Path.Combine(storepath, fileName);
                        userdetails.ProfilePicture.SaveAs(finalpath);

                        updatedetail.ProfilePicture = Path.Combine(("/Members/" + user.ID.ToString() + "/"), fileName);
                        objNotesEntities.SaveChanges();
                    }
                    else
                    {

                        updatedetail.ProfilePicture = Path.Combine("/SystemConfiguration/DefaultImage/", "DefaultUserImage.jpg");
                        objNotesEntities.SaveChanges();

                    }
                    objNotesEntities.Entry(user).State = EntityState.Modified;
                    objNotesEntities.SaveChanges();

                    objNotesEntities.Entry(updatedetail).State = EntityState.Modified;
                    objNotesEntities.SaveChanges();
                }
                else
                {
                    UserProfile updatedetail = new UserProfile
                    {

                        DateOfBirth = userdetails.DateOfBirth,
                        Gender = userdetails.Gender,
                        PhoneCountryCode = userdetails.PhoneCountryCode,
                        PhoneNumber = userdetails.PhoneNumber,
                        AddressLine1 = userdetails.AddressLine1,
                        AddressLine2 = userdetails.AddressLine2,
                        City = userdetails.City,
                        State = userdetails.State,
                        ZipCode = userdetails.ZipCode,
                        Country = userdetails.Country,
                        University = userdetails.University,
                        College = userdetails.College,
                    };
                    updatedetail.UserID = userdetails.UserID;
                    updatedetail.CreatedDate = DateTime.Now;
                    updatedetail.CreatedBy = userdetails.UserID;

                    string storepath = Path.Combine(Server.MapPath("~/Members/"), user.ID.ToString());

                    // Check for Directory, If not exist, then create it  
                    if (!Directory.Exists(storepath))
                    {
                        Directory.CreateDirectory(storepath);
                    }

                    if (userdetails.ProfilePicture != null && userdetails.ProfilePicture.ContentLength > 0)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(userdetails.ProfilePicture.FileName);
                        string extension = Path.GetExtension(userdetails.ProfilePicture.FileName);
                        fileName = "PP_" + DateTime.Now.ToString("ddMMyyyy") + extension;
                        string finalpath = Path.Combine(storepath, fileName);
                        userdetails.ProfilePicture.SaveAs(finalpath);

                        updatedetail.ProfilePicture = Path.Combine(("/Members/" + user.ID.ToString() + "/"), fileName);
                        objNotesEntities.SaveChanges();
                    }
                    else
                    {

                        updatedetail.ProfilePicture = Path.Combine("/SystemConfiguration/DefaultImage/", "DefaultUserImage.jpg");
                        objNotesEntities.SaveChanges();

                    }

                    objNotesEntities.UserProfile.Add(updatedetail);
                    objNotesEntities.SaveChanges();
                }
                return RedirectToAction("Dashboard", "Front");
            }
            return View();
        }


    }

}











/*




        [Route("UserProfile")]
        [HttpGet]
        public ActionResult UserProfile(UserProfile userdetail)
        {
            *//*            ViewBag.Country = objNotesEntities.Country.Where(x => x.IsActive == true);*//*

            var Emailid = User.Identity.Name.ToString();
            int id = Int16.Parse(Request.Cookies["ID"].Value);

            if (User.Identity.IsAuthenticated)
            {
                UserProfile Profilepic = objNotesEntities.UserProfile.Where(x => x.UserID == id).FirstOrDefault();
                if (Profilepic != null)
                {
                    TempData["ProfilePicture"] = Profilepic.ProfilePicture;
                }
                else
                {
                    TempData["ProfilePicture"] = Path.Combine("/SystemConfiguration/DefaultImage/", "DefaultUserImage.jpg");
                }
            }
            var v = objNotesEntities.Users.Where(x => x.EmailId == Emailid).FirstOrDefault();

            if (v != null)
            {
                UserProfileModel profile = new UserProfileModel
                {
                    FirstName = v.FirstName,
                    LastName = v.LastName,
                    EmailID = v.EmailId,
                    UserID = v.ID
                };

                UserProfile ud = objNotesEntities.UserProfile.Where(x => x.UserID == v.ID).FirstOrDefault();

                if (ud != null)
                {
                    profile.DateOfBirth = ud.DateOfBirth;
                    profile.Gender = ud.Gender;
                    profile.PhoneCountryCode = ud.PhoneCountryCode;
                    profile.PhoneNumber = ud.PhoneNumber;
                    profile.AddressLine1 = ud.AddressLine1;
                    profile.AddressLine2 = ud.AddressLine2;
                    profile.City = ud.City;
                    profile.State = ud.State;
                    profile.ZipCode = ud.ZipCode;
                    profile.Country = ud.Country;
                    profile.University = ud.University;
                    profile.College = ud.College;
                    *//*profile.ProfilePicture = ud.ProfilePicture;*//*
                    ViewBag.ProfilePicture = ud.ProfilePicture;
                    *//*ViewBag.ProfilePicture = Path.Combine("/SystemConfiguration/DefaultImage/", "DefaultUserImage.jpg");*//*
                    ViewBag.Country = new SelectList(objNotesEntities.Country.Distinct().Where(x => x.IsActive == true), "Name", "Name", profile.Country);
                    ViewBag.PhoneCountryCode = new SelectList(objNotesEntities.Country.Distinct().Where(x => x.IsActive == true), "CountryCode", "CountryCode", profile.PhoneCountryCode);
*//*                    ViewBag.Gender = new SelectList(objNotesEntities.ReferenceData.Where(x => x.RefCategory == "Gender" && x.IsActive == true), "ReferenceID", "Value", profile.Gender).ToList();
*//*                    return View(profile);
                }
                else
                {
                    *//*ViewBag.ProfilePicture = Path.Combine("/SystemConfiguration/DefaultImage/", "DefaultUserImage.jpg");*//*
                    ViewBag.Country = new SelectList(objNotesEntities.Country.Distinct().Where(x => x.IsActive == true), "Name", "Name");
                    ViewBag.PhoneCountryCode = new SelectList(objNotesEntities.Country.Distinct().Where(x => x.IsActive == true), "CountryCode", "CountryCode");
                    ViewBag.Gender = new SelectList(objNotesEntities.ReferenceData.Where(x => x.RefCategory == "Gender" && x.IsActive == true), "ReferenceID", "Value").ToList();
                    return View(profile);

                }

            }
            return View();

        }

        [HttpPost]
        public ActionResult UserProfile(UserProfileModel userdetails)
        {
            int id = Int16.Parse(Request.Cookies["ID"].Value);
            var Emailid = User.Identity.Name.ToString();


            if (ModelState.IsValid)
            {
                Users user = objNotesEntities.Users.Where(x => x.EmailId == Emailid).FirstOrDefault();

                user.FirstName = userdetails.FirstName;
                user.LastName = userdetails.LastName;


                if (id != null)
                {
                    UserProfile updatedetail = objNotesEntities.UserProfile.Where(x => x.UserID == userdetails.UserID).FirstOrDefault();
                    *//*Convert.ToDateTime(userdetails.DateOfBirth).ToString("yyyy-MM-dd")*//*
                    updatedetail.DateOfBirth = userdetails.DateOfBirth;
                    updatedetail.Gender = userdetails.Gender;
                    updatedetail.PhoneCountryCode = userdetails.PhoneCountryCode;
                    updatedetail.PhoneNumber = userdetails.PhoneNumber;
                    updatedetail.AddressLine1 = userdetails.AddressLine1;
                    updatedetail.AddressLine2 = userdetails.AddressLine2;
                    updatedetail.City = userdetails.City;
                    updatedetail.State = userdetails.State;
                    updatedetail.ZipCode = userdetails.ZipCode;
                    updatedetail.Country = userdetails.Country;
                    updatedetail.University = userdetails.University;
                    updatedetail.College = userdetails.College;
                    updatedetail.ModifiedBy = userdetails.UserID;
                    updatedetail.ModifiedDate = DateTime.Now;


                    string storepath = Path.Combine(Server.MapPath("~/Members/"), id.ToString());
                    System.IO.DirectoryInfo di = new DirectoryInfo(storepath);

                    *//* FileInfo file = new FileInfo(updatedetail.ProfilePicture);
                     if (file.Exists)//check file exsit or not  
                     {
                         file.Delete();

                     }*//*
                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }

                    if (!Directory.Exists(storepath))
                    {
                        Directory.CreateDirectory(storepath);
                    }

                    if (userdetails.ProfilePicture != null && userdetails.ProfilePicture.ContentLength > 0)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(userdetails.ProfilePicture.FileName);
                        string extension = Path.GetExtension(userdetails.ProfilePicture.FileName);
                        fileName = "DP_" + DateTime.Now.ToString("ddMMyyyy") + extension;
                        string finalpath = Path.Combine(storepath, fileName);
                        userdetails.ProfilePicture.SaveAs(finalpath);

                        updatedetail.ProfilePicture = Path.Combine(("/Members/" + id.ToString() + "/"), fileName);
                        objNotesEntities.SaveChanges();
                    }
                    else
                    {

                        updatedetail.ProfilePicture = Path.Combine("/SystemConfiguration/DefaultImage/", "DefaultUserImage.jpg");
                        objNotesEntities.SaveChanges();

                    }
                    objNotesEntities.Entry(user).State = EntityState.Modified;
                    objNotesEntities.SaveChanges();

                    objNotesEntities.Entry(updatedetail).State = EntityState.Modified;
                    objNotesEntities.SaveChanges();
                }


                else
                {


                    UserProfile updatedetail = new UserProfile
                    {

                        Gender = userdetails.Gender,
                        PhoneCountryCode = userdetails.PhoneCountryCode,
                        PhoneNumber = userdetails.PhoneNumber,
                        AddressLine1 = userdetails.AddressLine1,
                        AddressLine2 = userdetails.AddressLine2,
                        City = userdetails.City,
                        State = userdetails.State,
                        ZipCode = userdetails.ZipCode,
                        Country = userdetails.Country,
                        University = userdetails.University,
                        College = userdetails.College,

                    };
                    updatedetail.UserID = userdetails.UserID;
                    updatedetail.CreatedDate = DateTime.Now;
                    updatedetail.CreatedBy = userdetails.UserID;
                    *//*                objNotesEntities.UserProfile.Add(userdetails);
                                    objNotesEntities.SaveChanges();
                    *//*
                    string path = Path.Combine(Server.MapPath("~/Members/"), Request.Cookies["ID"].Value);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    if (userdetails.ProfilePicture != null && userdetails.ProfilePicture.ContentLength > 0)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(userdetails.ProfilePicture.FileName);
                        string extension = Path.GetExtension(userdetails.ProfilePicture.FileName);
                        fileName = "DP_" + DateTime.Now.ToString("ddMMyyyy") + extension;
                        string finalpath = Path.Combine(path, fileName);
                        userdetails.ProfilePicture.SaveAs(finalpath);

                        updatedetail.ProfilePicture = Path.Combine(("~/Members/" + Request.Cookies["ID"].Value + "/"), fileName);
                        objNotesEntities.SaveChanges();
                    }

                    objNotesEntities.UserProfile.Add(updatedetail);
                    objNotesEntities.SaveChanges();

                }
                    return RedirectToAction("Dashboard", "Front");
            }
            ModelState.Clear();
            return RedirectToAction("UserProfile", "Front");

        }






    }
}*/



