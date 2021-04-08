using Notes_MarketPlace.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Notes_MarketPlace.Helpers;

namespace Notes_MarketPlace.Controllers
{
    [Authorize(Roles = "Administrator,SuperAdmin")]
    [RoutePrefix("Admin")]
    public class AdminController : Controller
    {

        private NotesEntities objNotesEntities = new NotesEntities();

        [Route("Dashboard")]
        public ActionResult Dashboard(string SearchPublished, int? PublishedNotespage, string SortOrderPublished)
        {
            var EmailID = User.Identity.Name.ToString();
            Users userObj = objNotesEntities.Users.Where(x => x.EmailId == EmailID).FirstOrDefault();


            DateTime dtNow = DateTime.Now;

            var dt = DateTime.Now.AddDays(-7);

            var SpamReport = objNotesEntities.SellerNotesReportedIssues.Count();
            TempData["SpamReportsCount"] = SpamReport;
            TempData.Keep("SpamReportsCount");

            var Downloads = objNotesEntities.Downloads.Where(x => x.IsSellerHasAllowedDownload == true && x.AttachmentPath != null && x.IsAttachedDownloaded == true && x.CreatedDate > dt).Count();
            TempData["7DayDownloads"] = Downloads;

            var Registration = objNotesEntities.Users.Where(x => x.RoleID == (objNotesEntities.UserRole.Where(y => y.Name.ToLower() == "member").Select(y => y.ID).FirstOrDefault()) && x.CreateDate > dt).Count();
            TempData["7DayRegistrations"] = Registration;

            var UnderReviewNotes = objNotesEntities.SellerNotes.Where(x => x.Status == objNotesEntities.ReferenceData.Where(z => z.Value.ToLower() == "submitted for review").Select(y => y.ID).FirstOrDefault() ||
            x.Status == objNotesEntities.ReferenceData.Where(z => z.Value.ToLower() == "in review").Select(y => y.ID).FirstOrDefault()).Count();
            TempData["UnderReviewNotes"] = UnderReviewNotes;


            List<SellerNotes> NoteTitlePublished = objNotesEntities.SellerNotes.Where(x => x.IsActive == true && (x.NoteTitle.Contains(SearchPublished) || SearchPublished == null)).ToList();
            List<NoteCategories> CategoryNamePublished = objNotesEntities.NoteCategories.ToList();
            List<ReferenceData> StatusNamePublished = objNotesEntities.ReferenceData.Where(x => x.RefCategory == "Note Status" && x.Value != "Rejected" && x.Value != "Removed" && x.IsActive == true).ToList();
            List<Users> UserDetails = objNotesEntities.Users.ToList();
            List<SellerNotesAttachements> attachmentDetails = objNotesEntities.SellerNotesAttachements.ToList();

            ViewBag.DateSortParamPublish = string.IsNullOrEmpty(SortOrderPublished) ? "CreatedDate_asc" : "";
            ViewBag.TitleSortParamPublish = SortOrderPublished == "Title" ? "Title_desc" : "Title";
            ViewBag.CategorySortParamPublish = SortOrderPublished == "Category" ? "Category_desc" : "Category";
            ViewBag.SellTypeSortParamPublish = SortOrderPublished == "Type" ? "Type_desc" : "Type";
            ViewBag.PriceSortParamPublish = SortOrderPublished == "Price" ? "Price_desc" : "Price";

            ViewBag.SizeSortParamPublish = SortOrderPublished == "Size" ? "Size_desc" : "Size";
            ViewBag.PublisherSortParamPublish = SortOrderPublished == "Publisher" ? "Publisher_desc" : "Publisher";
            ViewBag.DownloadsSortParamPublish = SortOrderPublished == "Downloads" ? "Downloads_desc" : "Downloads";

            var PublishedNote = (from nt in NoteTitlePublished
                                 join cn in CategoryNamePublished on nt.NoteCategory equals cn.ID into table1
                                 from cn in table1.ToList()
                                 join sn in StatusNamePublished on nt.Status equals sn.ID into table2
                                 from sn in table2.ToList()
                                 join us in UserDetails on nt.SellerID equals us.ID into table3
                                 from us in table3.ToList()
                                 join ad in attachmentDetails on nt.ID equals ad.NoteID into table4
                                 from ad in table4.ToList()
                                 where sn.Value == "Published"

                                 select new AllPublishedNote
                                 {
                                     NoteDetails = nt,
                                     Category = cn,
                                     status = sn,
                                     user = us,
                                     attachment = ad
                                 }).AsQueryable();

            switch (SortOrderPublished)
            {
                case "CreatedDate_asc":
                    PublishedNote = PublishedNote.OrderBy(x => x.NoteDetails.PublishedDate);
                    break;
                case "Title_desc":
                    PublishedNote = PublishedNote.OrderByDescending(x => x.NoteDetails.NoteTitle);
                    break;
                case "Title":
                    PublishedNote = PublishedNote.OrderBy(x => x.NoteDetails.NoteTitle);
                    break;
                case "Category_desc":
                    PublishedNote = PublishedNote.OrderByDescending(x => x.Category.CategoryName);
                    break;
                case "Category":
                    PublishedNote = PublishedNote.OrderBy(x => x.Category.CategoryName);
                    break;
                case "Type_desc":
                    PublishedNote = PublishedNote.OrderByDescending(x => x.NoteDetails.isPaid);
                    break;
                case "Type":
                    PublishedNote = PublishedNote.OrderBy(x => x.NoteDetails.isPaid);
                    break;
                case "Price_desc":
                    PublishedNote = PublishedNote.OrderByDescending(x => x.NoteDetails.SellPrice);
                    break;
                case "Price":
                    PublishedNote = PublishedNote.OrderBy(x => x.NoteDetails.SellPrice);
                    break;
                case "Publisher_desc":
                    PublishedNote = PublishedNote.OrderByDescending(x => x.user.FirstName);
                    break;
                case "Publisher":
                    PublishedNote = PublishedNote.OrderBy(x => x.user.FirstName);
                    break;
                case "Downloads_desc":
                    PublishedNote = PublishedNote.OrderByDescending(x => x.NoteDetails.SellPrice);
                    break;
                case "Downloads":
                    PublishedNote = PublishedNote.OrderBy(x => x.NoteDetails.SellPrice);
                    break;
                default:
                    PublishedNote = PublishedNote.OrderByDescending(x => x.NoteDetails.PublishedDate);
                    break;
            }

            ViewBag.PublishedNote = PublishedNote.ToList().ToPagedList(PublishedNotespage ?? 1, 4);
            return View();
        }




        [Route("NotesUnderReview")]
        public ActionResult NotesUnderReview(string SearchUnderReview, int? NotesUnderReviewspage, string SortOrderUnderReview, string SellerName)
        {
            List<SellerNotes> NoteTitlePublished = objNotesEntities.SellerNotes.Where(x => x.IsActive == true && (x.NoteTitle.Contains(SearchUnderReview) || SearchUnderReview == null)).ToList();
            List<NoteCategories> CategoryNamePublished = objNotesEntities.NoteCategories.ToList();
            List<ReferenceData> StatusNamePublished = objNotesEntities.ReferenceData.Where(x => x.RefCategory == "Note Status" && x.Value != "Rejected" && x.Value != "Removed" && x.IsActive == true).ToList();
            List<Users> UserDetails = objNotesEntities.Users.ToList();


            ViewBag.DateSortParamPublish = string.IsNullOrEmpty(SortOrderUnderReview) ? "CreatedDate_asc" : "";
            ViewBag.TitleSortParamPublish = SortOrderUnderReview == "Title" ? "Title_desc" : "Title";
            ViewBag.CategorySortParamPublish = SortOrderUnderReview == "Category" ? "Category_desc" : "Category";

            ViewBag.SellerSortParamPublish = SortOrderUnderReview == "Seller" ? "Seller_desc" : "Seller";
            ViewBag.StatusSortParamPublish = SortOrderUnderReview == "Status" ? "Status_desc" : "Status";

            var NotesUnderReview = (from nt in NoteTitlePublished
                                    join cn in CategoryNamePublished on nt.NoteCategory equals cn.ID into table1
                                    from cn in table1.ToList()
                                    join sn in StatusNamePublished on nt.Status equals sn.ID into table2
                                    from sn in table2.ToList()
                                    join us in UserDetails on nt.SellerID equals us.ID into table3
                                    from us in table3.ToList()
                                    where ((sn.Value == "Submitted For Review" || sn.Value == "In Review" || sn.Value == "Draft") && ((us.FirstName + us.LastName) == SellerName || string.IsNullOrEmpty(SellerName)))
                                    select new NotesUnderReviewsNote
                                    {
                                        NoteDetails = nt,
                                        Category = cn,
                                        status = sn,
                                        user = us,

                                    }).AsQueryable();

            switch (SortOrderUnderReview)
            {
                case "CreatedDate_asc":
                    NotesUnderReview = NotesUnderReview.OrderBy(x => x.NoteDetails.ModifiedDate);
                    break;
                case "Title_desc":
                    NotesUnderReview = NotesUnderReview.OrderByDescending(x => x.NoteDetails.NoteTitle);
                    break;
                case "Title":
                    NotesUnderReview = NotesUnderReview.OrderBy(x => x.NoteDetails.NoteTitle);
                    break;
                case "Category_desc":
                    NotesUnderReview = NotesUnderReview.OrderByDescending(x => x.Category.CategoryName);
                    break;
                case "Category":
                    NotesUnderReview = NotesUnderReview.OrderBy(x => x.Category.CategoryName);
                    break;

                case "Seller_desc":
                    NotesUnderReview = NotesUnderReview.OrderByDescending(x => x.user.FirstName);
                    break;
                case "Seller":
                    NotesUnderReview = NotesUnderReview.OrderBy(x => x.user.FirstName);
                    break;
                case "Status_desc":
                    NotesUnderReview = NotesUnderReview.OrderByDescending(x => x.status.Value);
                    break;
                case "Status":
                    NotesUnderReview = NotesUnderReview.OrderBy(x => x.status.Value);
                    break;
                default:
                    NotesUnderReview = NotesUnderReview.OrderByDescending(x => x.NoteDetails.ModifiedDate);
                    break;
            }

            var Seller = objNotesEntities.Users.Where(x => x.IsEmailVerified == true && x.RoleID == 3)
    .Select(s => new
    {
        Text = s.FirstName + " " + s.LastName,
    }).Distinct().ToList();

            ViewBag.SellerName = new SelectList(Seller, "Text", "Text");

           ViewBag.NotesUnderReview = NotesUnderReview.ToList().ToPagedList(NotesUnderReviewspage ?? 1, 4);
            return View();
        }










        [Route("Approve/{id}")]
        [HttpGet]
        public ActionResult Approve(int? id)
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
                return HttpNotFound();
            }

            note.Status = objNotesEntities.ReferenceData.Where(x => x.RefCategory == "Note Status" && x.Value == "Published").Select(x => x.ID).FirstOrDefault();
            note.ModifiedBy = user.ID;
            note.ModifiedDate = DateTime.Now;
            note.ActionedBy = user.ID;
            note.PublishedDate = DateTime.Now;
            objNotesEntities.Entry(note).State = EntityState.Modified;
            objNotesEntities.SaveChanges();

            return RedirectToAction("NotesUnderReview", "Admin");
        }

        [Route("InReview/{id}")]
        [HttpGet]
        public ActionResult InReview(int? id)
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
                return HttpNotFound();
            }

            note.Status = objNotesEntities.ReferenceData.Where(x => x.RefCategory == "Note Status" && x.Value == "In Review").Select(x => x.ID).FirstOrDefault();
            note.ModifiedBy = user.ID;
            note.ModifiedDate = DateTime.Now;
            note.ActionedBy = user.ID;
            objNotesEntities.Entry(note).State = EntityState.Modified;
            objNotesEntities.SaveChanges();
            return RedirectToAction("NotesUnderReview", "Admin");
        }

        [Route("RejectedNote/{id}")]
        [HttpGet]
        public ActionResult RejectedNote(int? id, AdminRemarks adminremark)
        {
            var Emailid = User.Identity.Name.ToString();
            Users user = objNotesEntities.Users.Where(x => x.EmailId == Emailid).FirstOrDefault();


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SellerNotes note = objNotesEntities.SellerNotes.Find(id);

            if (note == null)
            {
                return HttpNotFound();
            }

            note.Status = objNotesEntities.ReferenceData.Where(x => x.RefCategory == "Note Status" && x.Value.ToLower() == "rejected").Select(x => x.ID).FirstOrDefault();
            note.ModifiedBy = user.ID;
            note.ModifiedDate = DateTime.Now;
            note.AdminRemarks = adminremark.Remarks;
            note.ActionedBy = user.ID;
            objNotesEntities.Entry(note).State = EntityState.Modified;
            objNotesEntities.SaveChanges();

            return RedirectToAction("NotesUnderReview", "Admin");
        }























[Route("NoteDetails_Admin/{id}")]
        public ActionResult NoteDetails_Admin(int? id)
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
            Country country = objNotesEntities.Country.Find(sellerNote.Country);
            NoteCategories category = objNotesEntities.NoteCategories.Find(sellerNote.NoteCategory);
            SellerNotesAttachements attechment = objNotesEntities.SellerNotesAttachements.Where(x => x.NoteID == sellerNote.ID).FirstOrDefault();
            if (country == null)
            {
                ViewBag.Country = null;
            }
            else
            {
                ViewBag.Country = country.CountryName;
            }
            var data = objNotesEntities.SellerNotes.Where(x => x.ID == id).FirstOrDefault();
            ViewBag.dispic = data.DisplayPicture;
            ViewBag.notepre = data.PreviewUpload;
            ViewBag.Category = category.CategoryName;
            ViewBag.Attchment = attechment.FilePath;

            return View(data);

        }































        [Route("PublishedNotes")]
        public ActionResult PublishedNotes(string Search, int? page, string SortOrder, string SellerName)
        {
            List<SellerNotes> NoteTitlePublished = objNotesEntities.SellerNotes.Where(x => x.IsActive == true && (x.NoteTitle.Contains(Search) || x.NoteCategories.CategoryName.Contains(Search) || x.SellPrice.ToString().StartsWith(Search) || x.Users.FirstName.Contains(Search) || x.Users.LastName.Contains(Search)
            || (x.PublishedDate.Value.Day + "-" + x.PublishedDate.Value.Month + "-" + x.PublishedDate.Value.Year).Contains(Search)
            || Search == null)).ToList();

            List<NoteCategories> CategoryNamePublished = objNotesEntities.NoteCategories.ToList();
            List<ReferenceData> StatusNamePublished = objNotesEntities.ReferenceData.Where(x => x.RefCategory == "Note Status" && x.Value != "Rejected" && x.Value != "Removed" && x.IsActive == true).ToList();
            List<Users> UserDetails = objNotesEntities.Users.ToList();

            ViewBag.DateSortParam = string.IsNullOrEmpty(SortOrder) ? "CreatedDate_asc" : "";
            ViewBag.TitleSortParam = SortOrder == "Title" ? "Title_desc" : "Title";
            ViewBag.CategorySortParam = SortOrder == "Category" ? "Category_desc" : "Category";
            ViewBag.SellerSortParam = SortOrder == "Seller" ? "Seller_desc" : "Seller";
            ViewBag.SellTypeSortParam = SortOrder == "SellType" ? "SellType_desc" : "SellType";
            ViewBag.PriceSortParam = SortOrder == "Price" ? "Price_desc" : "Price";
            ViewBag.ApprovedBySortParam = SortOrder == "ApprovedBy" ? "ApprovedBy_desc" : "ApprovedBy";

            var NotesPublished = (from nt in NoteTitlePublished
                                  join cn in CategoryNamePublished on nt.NoteCategory equals cn.ID into table1
                                  from cn in table1.ToList()
                                  join sn in StatusNamePublished on nt.Status equals sn.ID into table2
                                  from sn in table2.ToList()
                                  join us in UserDetails on nt.SellerID equals us.ID into table3
                                  from us in table3.ToList()
                                  join ad in UserDetails on nt.ActionedBy equals ad.ID into table4
                                  from ad in table4.ToList()
                                  where ((sn.Value == "Published") && ((us.FirstName + us.LastName) == SellerName || string.IsNullOrEmpty(SellerName)))
                                  select new PublishedNoteAdmin
                                  {
                                      NoteDetails = nt,
                                      Category = cn,
                                      status = sn,
                                      user = us,
                                      admin = ad
                                  }).AsQueryable();

            switch (SortOrder)
            {
                case "CreatedDate_asc":
                    NotesPublished = NotesPublished.OrderBy(x => x.NoteDetails.PublishedDate);
                    break;
                case "Title_desc":
                    NotesPublished = NotesPublished.OrderByDescending(x => x.NoteDetails.NoteTitle);
                    break;
                case "Title":
                    NotesPublished = NotesPublished.OrderBy(x => x.NoteDetails.NoteTitle);
                    break;
                case "Category_desc":
                    NotesPublished = NotesPublished.OrderByDescending(x => x.Category.CategoryName);
                    break;
                case "Category":
                    NotesPublished = NotesPublished.OrderBy(x => x.Category.CategoryName);
                    break;
                case "Seller_desc":
                    NotesPublished = NotesPublished.OrderByDescending(x => x.user.FirstName);
                    break;
                case "Seller":
                    NotesPublished = NotesPublished.OrderBy(x => x.user.FirstName);
                    break;
                case "SellType_desc":
                    NotesPublished = NotesPublished.OrderByDescending(x => x.NoteDetails.isPaid);
                    break;
                case "SellType":
                    NotesPublished = NotesPublished.OrderBy(x => x.NoteDetails.isPaid);
                    break;
                case "Price_desc":
                    NotesPublished = NotesPublished.OrderByDescending(x => x.NoteDetails.SellPrice);
                    break;
                case "Price":
                    NotesPublished = NotesPublished.OrderBy(x => x.NoteDetails.SellPrice);
                    break;
                case "ApprovedBy_desc":
                    NotesPublished = NotesPublished.OrderByDescending(x => x.user.FirstName);
                    break;
                case "ApprovedBy":
                    NotesPublished = NotesPublished.OrderBy(x => x.user.FirstName);
                    break;
                default:
                    NotesPublished = NotesPublished.OrderByDescending(x => x.NoteDetails.PublishedDate);
                    break;
            }

            var Seller = objNotesEntities.Users.Where(x => x.IsEmailVerified == true && x.RoleID == 3 && x.IsActive == true)
    .Select(s => new
    {
        Text = s.FirstName + " " + s.LastName,
    }).Distinct().ToList();

            ViewBag.SellerName = new SelectList(Seller, "Text", "Text");
           ViewBag.NotesPublished = NotesPublished.ToList().ToPagedList(page ?? 1, 4);
            return View();
        }


        [Route("DownloadedNotes")]
        public ActionResult DownloadedNotes(int? page, string SellerName, string BuyerName, string Search, string AllNotes, string SortOrder)
        {
            List<Downloads> downloads = objNotesEntities.Downloads.Where(x => x.IsActive == true && x.IsSellerHasAllowedDownload == true && x.AttachmentPath != null && x.IsAttachedDownloaded == true && (x.NoteTitle.Contains(Search) || x.NoteCategory.Contains(Search) || x.PurchasedPrice.ToString().StartsWith(Search) || x.Users.FirstName.Contains(Search) || x.Users.LastName.Contains(Search)
            || (x.AttachmentDownloadedDate.Value.Day + "-" + x.AttachmentDownloadedDate.Value.Month + "-" + x.AttachmentDownloadedDate.Value.Year).Contains(Search)
            || Search == null)).ToList();
            List<Users> users = objNotesEntities.Users.Where(x => x.RoleID == objNotesEntities.UserRole.Where(y => y.Name.ToLower() == "Member").Select(y => y.ID).FirstOrDefault() && x.IsEmailVerified == true && x.IsActive == true).ToList();

            ViewBag.DateSortParam = string.IsNullOrEmpty(SortOrder) ? "CreatedDate_asc" : "";
            ViewBag.TitleSortParam = SortOrder == "Title" ? "Title_desc" : "Title";
            ViewBag.CategorySortParam = SortOrder == "Category" ? "Category_desc" : "Category";
            ViewBag.SellerSortParam = SortOrder == "Seller" ? "Seller_desc" : "Seller";
            ViewBag.BuyerSortParam = SortOrder == "Buyer" ? "Buyer_desc" : "Buyer";
            ViewBag.SellTypeSortParam = SortOrder == "SellType" ? "SellType_desc" : "SellType";
            ViewBag.PriceSortParam = SortOrder == "Price" ? "Price_desc" : "Price";

            var downloadsnotes = (from nt in downloads
                                  join sell in users on nt.Seller equals sell.ID into table1
                                  from sell in table1.ToList()
                                  join down in users on nt.Downloader equals down.ID into table2
                                  from down in table2.ToList()
                                  where (((sell.FirstName + sell.LastName) == SellerName || string.IsNullOrEmpty(SellerName)) &&
                                  ((down.FirstName + down.LastName) == BuyerName || string.IsNullOrEmpty(BuyerName)) &&
                                  ((nt.NoteTitle) == AllNotes || string.IsNullOrEmpty(AllNotes))
                                  )
                                  select new AdminDownloadNotes
                                  {
                                      downloads = nt,
                                      seller = sell,
                                      buyer = down,
                                  }).AsQueryable();

            var Seller = objNotesEntities.Users.Where(x => x.IsEmailVerified == true && x.RoleID == 3 && x.IsActive == true)
   .Select(s => new
   {
       Text = s.FirstName + "" + s.LastName,
   }).Distinct().ToList();

            var Notes = objNotesEntities.Downloads.Where(x => x.IsActive == true)
   .Select(s => new
   {
       Text = s.NoteTitle,
   }).Distinct().ToList();

            ViewBag.SellerName = new SelectList(Seller, "Text", "Text");
            ViewBag.BuyerName = new SelectList(Seller, "Text", "Text");
            ViewBag.AllNotes = new SelectList(Notes, "Text", "Text");


            switch (SortOrder)
            {
                case "CreatedDate_asc":
                    downloadsnotes = downloadsnotes.OrderBy(x => x.downloads.AttachmentDownloadedDate);
                    break;
                case "Title_desc":
                    downloadsnotes = downloadsnotes.OrderByDescending(x => x.downloads.NoteTitle);
                    break;
                case "Title":
                    downloadsnotes = downloadsnotes.OrderBy(x => x.downloads.NoteTitle);
                    break;
                case "Category_desc":
                    downloadsnotes = downloadsnotes.OrderByDescending(x => x.downloads.NoteCategory);
                    break;
                case "Category":
                    downloadsnotes = downloadsnotes.OrderBy(x => x.downloads.NoteCategory);
                    break;
                case "Seller_desc":
                    downloadsnotes = downloadsnotes.OrderByDescending(x => x.seller.FirstName);
                    break;
                case "Seller":
                    downloadsnotes = downloadsnotes.OrderBy(x => x.seller.FirstName);
                    break;
                case "Buyer_desc":
                    downloadsnotes = downloadsnotes.OrderByDescending(x => x.buyer.FirstName);
                    break;
                case "Buyer":
                    downloadsnotes = downloadsnotes.OrderBy(x => x.buyer.FirstName);
                    break;
                case "SellType_desc":
                    downloadsnotes = downloadsnotes.OrderByDescending(x => x.downloads.IsPaid);
                    break;
                case "SellType":
                    downloadsnotes = downloadsnotes.OrderBy(x => x.downloads.IsPaid);
                    break;
                case "Price_desc":
                    downloadsnotes = downloadsnotes.OrderByDescending(x => x.downloads.PurchasedPrice);
                    break;
                case "Price":
                    downloadsnotes = downloadsnotes.OrderBy(x => x.downloads.PurchasedPrice);
                    break;

                default:
                    downloadsnotes = downloadsnotes.OrderByDescending(x => x.downloads.AttachmentDownloadedDate);
                    break;
            }

            ViewBag.downloadsnotes = downloadsnotes.ToList().ToPagedList(page ?? 1, 5);
            return View();
        }


        [Route("RejectedNotes")]
        public ActionResult RejectedNotes(string Search, int? page, string SortOrder, string SellerName)
        {
            List<SellerNotes> NoteTitlePublished = objNotesEntities.SellerNotes.Where(x => x.IsActive == true && (x.NoteTitle.Contains(Search) || x.NoteCategories.CategoryName.Contains(Search) || x.Users.FirstName.Contains(Search)
            || (x.ModifiedDate.Value.Day + "-" + x.ModifiedDate.Value.Month + "-" + x.ModifiedDate.Value.Year).Contains(Search)
            || x.AdminRemarks.Contains(Search) || Search == null)).ToList();
            List<NoteCategories> CategoryNamePublished = objNotesEntities.NoteCategories.ToList();
            List<ReferenceData> StatusNamePublished = objNotesEntities.ReferenceData.Where(x => x.RefCategory == "Note Status" && x.Value != "Removed" && x.IsActive == true).ToList();
            List<Users> UserDetails = objNotesEntities.Users.ToList();


            ViewBag.DateSortParam = string.IsNullOrEmpty(SortOrder) ? "CreatedDate_asc" : "";
            ViewBag.TitleSortParam = SortOrder == "Title" ? "Title_desc" : "Title";
            ViewBag.CategorySortParam = SortOrder == "Category" ? "Category_desc" : "Category";

            ViewBag.SellerSortParam = SortOrder == "Seller" ? "Seller_desc" : "Seller";
            ViewBag.RejectedBySortParam = SortOrder == "RejectedBy" ? "RejectedBy_desc" : "RejectedBy";

            var NotesRejected = (from nt in NoteTitlePublished
                                 join cn in CategoryNamePublished on nt.NoteCategory equals cn.ID into table1
                                 from cn in table1.ToList()
                                 join sn in StatusNamePublished on nt.Status equals sn.ID into table2
                                 from sn in table2.ToList()
                                 join us in UserDetails on nt.SellerID equals us.ID into table3
                                 from us in table3.ToList()
                                 join ad in UserDetails on nt.ActionedBy equals ad.ID into table4
                                 from ad in table4.ToList()
                                 where ((sn.Value == "Rejected") && ((us.FirstName + us.LastName) == SellerName || string.IsNullOrEmpty(SellerName)))
                                 select new RejectedNoteAdmin
                                 {
                                     NoteDetails = nt,
                                     Category = cn,
                                     status = sn,
                                     user = us,
                                     admin = ad
                                 }).AsQueryable();

            switch (SortOrder)
            {
                case "CreatedDate_asc":
                    NotesRejected = NotesRejected.OrderBy(x => x.NoteDetails.ModifiedDate);
                    break;
                case "Title_desc":
                    NotesRejected = NotesRejected.OrderByDescending(x => x.NoteDetails.NoteTitle);
                    break;
                case "Title":
                    NotesRejected = NotesRejected.OrderBy(x => x.NoteDetails.NoteTitle);
                    break;
                case "Category_desc":
                    NotesRejected = NotesRejected.OrderByDescending(x => x.Category.CategoryName);
                    break;
                case "Category":
                    NotesRejected = NotesRejected.OrderBy(x => x.Category.CategoryName);
                    break;

                case "Seller_desc":
                    NotesRejected = NotesRejected.OrderByDescending(x => x.user.FirstName);
                    break;
                case "Seller":
                    NotesRejected = NotesRejected.OrderBy(x => x.user.FirstName);
                    break;
                case "RejectedBy_desc":
                    NotesRejected = NotesRejected.OrderByDescending(x => x.admin.FirstName);
                    break;
                case "RejectedBy":
                    NotesRejected = NotesRejected.OrderBy(x => x.admin.FirstName);
                    break;
                default:
                    NotesRejected = NotesRejected.OrderByDescending(x => x.NoteDetails.ModifiedDate);
                    break;
            }

            var Seller = objNotesEntities.Users.Where(x => x.IsEmailVerified == true && x.RoleID == 3 && x.IsActive == true)
    .Select(s => new
    {
        Text = s.FirstName + "" + s.LastName,
    }).Distinct().ToList();

            ViewBag.SellerName = new SelectList(Seller, "Text", "Text");
          ViewBag.NotesRejected = NotesRejected.ToList().ToPagedList(page ?? 1, 5);
            return View();
        }

        [Route("ApproveRejected/{id}")]
        [HttpGet]
        public ActionResult ApproveRejected(int? id)
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

            note.Status = objNotesEntities.ReferenceData.Where(x => x.RefCategory == "Note Status" && x.Value == "Published").Select(x => x.ID).FirstOrDefault();
            note.ModifiedBy = user.ID;
            note.ModifiedDate = DateTime.Now;
            note.ActionedBy = user.ID;
            note.PublishedDate = DateTime.Now;
            note.AdminRemarks = null;
            objNotesEntities.Entry(note).State = EntityState.Modified;
            objNotesEntities.SaveChanges();


            TempData["RejectedApprove"] = user.FirstName + " " + user.LastName;
            TempData["title"] = note.NoteTitle;
            TempData["Message"] = ",Approved Successfully !";

            return RedirectToAction("NotesRejected", "Admin");
        }

































        [Route("SpamReports")]
        public ActionResult SpamReports(string Search, int? page, string SortOrder)
        {
            var name = objNotesEntities.Users.Select(x => x.FirstName).ToList();

            List<SellerNotesReportedIssues> reports = objNotesEntities.SellerNotesReportedIssues.Where(x => (x.Users.FirstName.Contains(Search) || x.Users.LastName.Contains(Search) || x.Remarks.Contains(Search)
            || x.SellerNotes.NoteTitle.Contains(Search) || x.SellerNotes.NoteCategories.CategoryName.Contains(Search)
           || (x.ModifiedDate.Value.Day + "-" + x.ModifiedDate.Value.Month + "-" + x.ModifiedDate.Value.Year).Contains(Search)
            || Search == null)).ToList();
            List<SellerNotes> NoteTitlePublished = objNotesEntities.SellerNotes.Where(x => x.IsActive == true).ToList();
            List<NoteCategories> CategoryNamePublished = objNotesEntities.NoteCategories.ToList();
            List<Users> UserDetails = objNotesEntities.Users.ToList();

            var SpamReport = objNotesEntities.SellerNotesReportedIssues.Count();
            TempData["SpamReportsCount"] = SpamReport;
            TempData.Keep("SpamReportsCount");

            ViewBag.DateSortParam = string.IsNullOrEmpty(SortOrder) ? "CreatedDate_asc" : "";
            ViewBag.TitleSortParam = SortOrder == "Title" ? "Title_desc" : "Title";
            ViewBag.CategorySortParam = SortOrder == "Category" ? "Category_desc" : "Category";
            ViewBag.ReportedBySortParam = SortOrder == "ReportedBy" ? "ReportedBy_desc" : "ReportedBy";

            var NotesSpamReport = (from sr in reports
                                   join nt in NoteTitlePublished on sr.NoteID equals nt.ID into table1
                                   from nt in table1.ToList()
                                   join cn in CategoryNamePublished on nt.NoteCategory equals cn.ID into table2
                                   from cn in table2.ToList()
                                   join us in UserDetails on sr.ReportedByID equals us.ID into table3
                                   from us in table3.ToList()
                                   select new SpamReportsmodel
                                   {
                                       Reports = sr,
                                       NoteDetails = nt,
                                       Category = cn,
                                       user = us
                                   }).AsQueryable();


            switch (SortOrder)
            {
                case "CreatedDate_asc":
                    NotesSpamReport = NotesSpamReport.OrderBy(x => x.Reports.ModifiedDate);
                    break;
                case "Title_desc":
                    NotesSpamReport = NotesSpamReport.OrderByDescending(x => x.NoteDetails.NoteTitle);
                    break;
                case "Title":
                    NotesSpamReport = NotesSpamReport.OrderBy(x => x.NoteDetails.NoteTitle);
                    break;
                case "Category_desc":
                    NotesSpamReport = NotesSpamReport.OrderByDescending(x => x.Category.CategoryName);
                    break;
                case "Category":
                    NotesSpamReport = NotesSpamReport.OrderBy(x => x.Category.CategoryName);
                    break;
                case "ReportedBy_desc":
                    NotesSpamReport = NotesSpamReport.OrderByDescending(x => x.user.FirstName);
                    break;
                case "ReportedBy":
                    NotesSpamReport = NotesSpamReport.OrderBy(x => x.user.FirstName);
                    break;
                default:
                    NotesSpamReport = NotesSpamReport.OrderByDescending(x => x.Reports.ModifiedDate);
                    break;
            }

            ViewBag.NotesSpamReport = NotesSpamReport.ToList().ToPagedList(page ?? 1, 5);
            return View();
        }

        [Route("DeleteSpamReport/{id}")]
        public ActionResult DeleteSpamReport(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SellerNotesReportedIssues report = objNotesEntities.SellerNotesReportedIssues.Find(id);
            if (report == null)
            {
                return RedirectToAction("Error", "HomePage");
            }
            objNotesEntities.SellerNotesReportedIssues.Remove(report);
            objNotesEntities.SaveChanges();

            var Emailid = User.Identity.Name.ToString();
            Users user = objNotesEntities.Users.Where(x => x.EmailId == Emailid).FirstOrDefault();
            TempData["SpamReport"] = user.FirstName + " " + user.LastName;

            TempData["Message"] = ",Deleted Successfully !";

            return RedirectToAction("SpamReports", "Admin");
        }




        [Route("MyProfile")]
        [HttpGet]
        public ActionResult MyProfile()
        {
            ViewBag.Title = "My Profile";
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
                AdminUserProfileModel profile = new AdminUserProfileModel
                {
                    FirstName = v.FirstName,
                    LastName = v.LastName,
                    EmailID = v.EmailId,
                    ID = v.ID
                };

                UserProfile ud = objNotesEntities.UserProfile.Where(x => x.UserID == v.ID).FirstOrDefault();

                if (ud != null)
                {
                    profile.SecondaryEmail = ud.SecondaryEmail;
                    profile.PhoneCountryCode = ud.PhoneCountryCode;
                    profile.PhoneNumber = ud.PhoneNumber;
                    ViewBag.ProfilePicture = ud.ProfilePicture;

                  
                    ViewBag.PhoneCountryCode = new SelectList(objNotesEntities.Country.Distinct().Where(x => x.IsActive == true), "CountryCode", "CountryCode", profile.PhoneCountryCode);

                    return View(profile);
                }
                else
                {
                    ViewBag.Country = new SelectList(objNotesEntities.Country.Distinct().Where(x => x.IsActive == true), "Name", "Name");
                    ViewBag.PhoneCountryCode = new SelectList(objNotesEntities.Country.Distinct().Where(x => x.IsActive == true), "CountryCode", "CountryCode");
                    return View(profile);

                }

            }
            return View();

        }

        [Route("MyProfile")]
        [HttpPost]
        public ActionResult MyProfile(AdminUserProfileModel userdetails)
        {
            if (ModelState.IsValid)
            {
                var Emailid = User.Identity.Name.ToString();

                Users user = objNotesEntities.Users.Where(x => x.EmailId == Emailid).FirstOrDefault();

                user.FirstName = userdetails.FirstName;
                user.LastName = userdetails.LastName;


                var id = objNotesEntities.UserProfile.Where(x => x.UserID == userdetails.UserID).FirstOrDefault();

                if (id != null)
                {
                    UserProfile updatedetail = objNotesEntities.UserProfile.Where(x => x.UserID == userdetails.UserID).FirstOrDefault();
                    
                    updatedetail.ModifiedBy = userdetails.UserID;
                    updatedetail.ModifiedDate = DateTime.Now;

                    string storepath = Path.Combine(Server.MapPath("~/Members/"), user.ID.ToString());
                    System.IO.DirectoryInfo di = new DirectoryInfo(storepath);

                    if (di == null)
                    {
                        foreach (FileInfo file in di.GetFiles())
                        {
                            file.Delete();
                        }
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

                        SecondaryEmail = userdetails.SecondaryEmail,
                        PhoneCountryCode = userdetails.PhoneCountryCode,
                        PhoneNumber = userdetails.PhoneNumber,
                       
                    };
                    updatedetail.UserID = userdetails.UserID;
                    updatedetail.CreatedDate = DateTime.Now;
                    updatedetail.CreatedBy = userdetails.UserID;

                    string storepath = Path.Combine(Server.MapPath("~/Members/"), user.ID.ToString());

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
                return RedirectToAction("Dashboard", "Admin");
            }
            return View();
        }








        [Route("Unpublished/{id}")]
        [HttpGet]
        public ActionResult Unpublished(int? id, UnpublishedNote unpublished)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            bool internet = InternetConnection.IsConnectedToInternet();
            if (internet != true)
            {
                var Emailid = User.Identity.Name.ToString();
                Users user2 = objNotesEntities.Users.Where(x => x.EmailId == Emailid).FirstOrDefault();
                TempData["internetnotconnected"] = user2.FirstName + " " + user2.LastName;
                return RedirectToAction("Dashboard", "Member");
            }

            SellerNotes note = objNotesEntities.SellerNotes.Where(x => x.ID == id).FirstOrDefault();

            Users user = objNotesEntities.Users.Find(note.SellerID);

            note.Status = objNotesEntities.ReferenceData.Where(x => x.RefCategory == "Note Status" && x.Value.ToLower() == "removed").Select(x => x.ID).FirstOrDefault();
            note.ModifiedBy = user.ID;
            note.ModifiedDate = DateTime.Now;
            objNotesEntities.Entry(note).State = EntityState.Modified;
            objNotesEntities.SaveChanges();

            string Remarks = unpublished.Remarks;


            TempData["AdminDashboard"] = user.FirstName + " " + user.LastName;
            TempData["Message"] = "Your Email has been Successfully Sent For UnPublished Note.";

            return RedirectToAction("Dashboard", "Admin");
        }


    }
}