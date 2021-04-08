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

namespace Notes_MarketPlace.Controllers
{
    [Authorize(Roles = "Administrator,SuperAdmin")]
    [RoutePrefix("Admin")]
    public class AdminMemberController : Controller
    {
        private NotesEntities objNotesEntities = new NotesEntities();

        [Route("MembersAdmin")]
        public ActionResult Members(int? page, string SortOrder, string Search)
        {
            var users = objNotesEntities.Users.Where(x => x.RoleID == objNotesEntities.UserRole.Where(y => y.Name.ToLower() == "member").Select(y => y.ID).FirstOrDefault() && x.IsEmailVerified == true && x.IsActive == true && ((x.FirstName.Contains(Search) || x.LastName.Contains(Search) || x.EmailId.Contains(Search)
            || (x.CreateDate.Value.Day + "-" + x.CreateDate.Value.Month + "-" + x.CreateDate.Value.Year).Contains(Search)
            || x.Downloads1.Where(y => y.Downloader == x.ID && y.IsSellerHasAllowedDownload == true && y.AttachmentPath != null).Select(y => y.PurchasedPrice).Sum().ToString().StartsWith(Search)
            || x.Downloads.Where(y => y.Seller == x.ID && y.IsSellerHasAllowedDownload == true && y.AttachmentPath != null).Select(y => y.PurchasedPrice).Sum().ToString().StartsWith(Search)
            || Search == null)));

  
            ViewBag.DateSortParam = string.IsNullOrEmpty(SortOrder) ? "CreatedDate_asc" : "";
            ViewBag.FirstNameSortParam = SortOrder == "FirstName" ? "FirstName_desc" : "FirstName";
            ViewBag.LastNameSortParam = SortOrder == "LastName" ? "LastName_desc" : "LastName";
            ViewBag.EmailIDSortParam = SortOrder == "EmailID" ? "EmailID_desc" : "EmailID";

            ViewBag.Download = objNotesEntities.Downloads.Where(x => x.IsSellerHasAllowedDownload == true && x.AttachmentPath != null && x.IsAttachedDownloaded == true).ToList();
            ViewBag.status = objNotesEntities.ReferenceData.Where(x => x.IsActive == true).ToList();

            switch (SortOrder)
            {
                case "CreatedDate_asc":
                    users = users.OrderBy(x => x.CreateDate);
                    break;
                case "FirstName_desc":
                    users = users.OrderByDescending(x => x.FirstName);
                    break;
                case "FirstName":
                    users = users.OrderBy(x => x.FirstName);
                    break;
                case "LastName_desc":
                    users = users.OrderByDescending(x => x.LastName);
                    break;
                case "LastName":
                    users = users.OrderBy(x => x.LastName);
                    break;
                case "EmailID_desc":
                    users = users.OrderByDescending(x => x.EmailId);
                    break;
                case "EmailID":
                    users = users.OrderBy(x => x.EmailId);
                    break;
                default:
                    users = users.OrderByDescending(x => x.CreateDate);
                    break;
            }

            return View(users.ToList().ToPagedList(page ?? 1, 5));
        }

        [Route("MemberDetails/{id}")]
        public ActionResult MemberDetails(int? id, string SortOrderPublished, int? PublishedNotespage, string SearchPublished)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UserProfile memberdetails = objNotesEntities.UserProfile.Where(x => x.UserID == id).FirstOrDefault();

            if (memberdetails == null)
            {
                var name = objNotesEntities.Users.Where(x => x.ID == id).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();
                TempData["profile"] = name;
                if (TempData["profile"] != null)
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                else
                {
                    return RedirectToAction("Error", "HomePage");
                }
            }

          

            Users user = objNotesEntities.Users.Find(id);
            ViewBag.User = user;
          
            ViewBag.MemberDetails = memberdetails;

      
            List<SellerNotes> NoteTitlePublished = objNotesEntities.SellerNotes.Where(x => x.SellerID == id && x.IsActive == true && (x.NoteTitle.Contains(SearchPublished) || x.NoteCategories.CategoryName.Contains(SearchPublished) || x.ReferenceData.Value.Contains(SearchPublished)
             || (x.PublishedDate.Value.Day + "-" + x.PublishedDate.Value.Month + "-" + x.PublishedDate.Value.Year).Contains(SearchPublished)
              || (x.ModifiedDate.Value.Day + "-" + x.ModifiedDate.Value.Month + "-" + x.ModifiedDate.Value.Year).Contains(SearchPublished)
            || SearchPublished == null)).ToList();

            List<NoteCategories> CategoryNamePublished = objNotesEntities.NoteCategories.ToList();
            List<ReferenceData> StatusNamePublished = objNotesEntities.ReferenceData.Where(x => x.RefCategory == "Note Status" && x.IsActive == true).ToList();
            List<Downloads> DonwloadsNote = objNotesEntities.Downloads.ToList();
            ViewBag.DateSortParamPublish = string.IsNullOrEmpty(SortOrderPublished) ? "CreatedDate_asc" : "";
            ViewBag.TitleSortParamPublish = SortOrderPublished == "Title" ? "Title_desc" : "Title";
            ViewBag.CategorySortParamPublish = SortOrderPublished == "Category" ? "Category_desc" : "Category";
            ViewBag.StatusSortParamPublish = SortOrderPublished == "Status" ? "Status_desc" : "Status";
            ViewBag.DownloadSortParamPublish = SortOrderPublished == "Download" ? "Download_desc" : "Download";
            ViewBag.EarningSortParamPublish = SortOrderPublished == "Earning" ? "Earning_desc" : "Earning";
            ViewBag.PublishedDateSortParamPublish = SortOrderPublished == "PublishedDate" ? "PublishedDate_desc" : "PublishedDate";

            var MemberAllNotes = (from nt in NoteTitlePublished
                                  join cn in CategoryNamePublished on nt.NoteCategory equals cn.ID into table1
                                  from cn in table1.ToList()
                                  join sn in StatusNamePublished on nt.Status equals sn.ID into table2
                                  from sn in table2.ToList()
                                     
                                  where sn.Value != "Draft"
                                  select new MemberNotes
                                  {
                                      NoteDetails = nt,
                                      Category = cn,
                                      status = sn
                                  }).AsQueryable();

            switch (SortOrderPublished)
            {
                case "CreatedDate_asc":
                    MemberAllNotes = MemberAllNotes.OrderBy(x => x.NoteDetails.ModifiedDate);
                    break;
                case "Title_desc":
                    MemberAllNotes = MemberAllNotes.OrderByDescending(x => x.NoteDetails.NoteTitle);
                    break;
                case "Title":
                    MemberAllNotes = MemberAllNotes.OrderBy(x => x.NoteDetails.NoteTitle);
                    break;
                case "Category_desc":
                    MemberAllNotes = MemberAllNotes.OrderByDescending(x => x.Category.CategoryName);
                    break;
                case "Category":
                    MemberAllNotes = MemberAllNotes.OrderBy(x => x.Category.CategoryName);
                    break;
                case "Status_desc":
                    MemberAllNotes = MemberAllNotes.OrderByDescending(x => x.status.Value);
                    break;
                case "Status":
                    MemberAllNotes = MemberAllNotes.OrderBy(x => x.status.Value);
                    break;
                case "Download_desc":
                    MemberAllNotes = MemberAllNotes.OrderByDescending(x => x.status.Value);
                    break;
                case "Download":
                    MemberAllNotes = MemberAllNotes.OrderBy(x => x.status.Value);
                    break;
                case "Earning_desc":
                    MemberAllNotes = MemberAllNotes.OrderByDescending(x => x.status.Value);
                    break;
                case "Earning":
                    MemberAllNotes = MemberAllNotes.OrderBy(x => x.status.Value);
                    break;
                case "PublishedDate_desc":
                    MemberAllNotes = MemberAllNotes.OrderByDescending(x => x.NoteDetails.PublishedDate);
                    break;
                case "PublishedDate":
                    MemberAllNotes = MemberAllNotes.OrderBy(x => x.NoteDetails.PublishedDate);
                    break;
                default:
                    MemberAllNotes = MemberAllNotes.OrderByDescending(x => x.NoteDetails.ModifiedDate);
                    break;
            }
            ViewBag.MemberAllNotes = MemberAllNotes.ToList().ToPagedList(PublishedNotespage ?? 1, 5);
            return View();
        }


        [Route("DeactivateMember/{id}")]
        public ActionResult DeactivateMember(int? id)
        {
            var Emailid = User.Identity.Name.ToString();
            Users user = objNotesEntities.Users.Where(x => x.EmailId == Emailid).FirstOrDefault();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = objNotesEntities.Users.Find(id);

            users.IsActive = false;
            objNotesEntities.Entry(users).State = EntityState.Modified;
            objNotesEntities.SaveChanges();

            var userid = users.ID;

            var notes = objNotesEntities.SellerNotes.Where(x => x.SellerID == userid);
            foreach (var item in notes)
            {
                item.Status = objNotesEntities.ReferenceData.Where(x => x.RefCategory == "Note Status" && x.Value.ToLower() == "removed").Select(x => x.ID).FirstOrDefault();
                item.ActionedBy = user.ID;
                item.ModifiedBy = user.ID;
                item.ModifiedDate = DateTime.Now;
                item.AdminRemarks = null;
                objNotesEntities.Entry(item).State = EntityState.Modified;
            }
            objNotesEntities.SaveChanges();

            TempData["Deactivate"] = users.FirstName + " " + users.LastName;
            TempData["Message"] = "Deactivate Successfully !";

            return RedirectToAction("MembersAdmin", "Admin");

        }

    }
}