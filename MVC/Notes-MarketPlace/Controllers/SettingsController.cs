using Notes_MarketPlace.Models;
using System;
using System.IO;
using System.Linq;
using System.Web;
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
    [Authorize(Roles = "Administrator,SuperAdmin")]
    [RoutePrefix("Admin")]
    public class SettingsController : Controller
    {


        private NotesEntities objNotesEntities = new NotesEntities();



        public ActionResult ManageSystemConfiguration()
        {
            return View();
        }





        [Route("ManageAdministrator")]
        public ActionResult ManageAdministrator(int? page, string Search, string SortOrder)
        {
            ViewBag.DateSortParam = string.IsNullOrEmpty(SortOrder) ? "CreatedDate_asc" : "";
            ViewBag.FirstNameSortParam = SortOrder == "FirstName" ? "FirstName_desc" : "FirstName";
            ViewBag.LastNameSortParam = SortOrder == "LastName" ? "LastName_desc" : "LastName";
            ViewBag.EmailIDSortParam = SortOrder == "EmailID" ? "EmailID_desc" : "EmailID";
            ViewBag.ActiveSortParam = SortOrder == "Active" ? "Active_desc" : "Active";

            var admin = objNotesEntities.Users.Where(x => x.RoleID == objNotesEntities.UserRole.Where(y => y.Name.ToLower() == "administrator").Select(z => z.ID).FirstOrDefault() && (x.FirstName.Contains(Search) || x.LastName.Contains(Search) || x.EmailId.Contains(Search)
            || (x.ModifiedDate.Value.Day + "-" + x.ModifiedDate.Value.Month + "-" + x.ModifiedDate.Value.Year).Contains(Search)
            || x.UserProfile.Select(y => y.PhoneCountryCode).ToList().Contains(Search) || x.UserProfile.All(y => y.PhoneNumber.Contains(Search)) || x.UserProfile.Select(y => y.PhoneNumber).ToList().Contains(Search) || Search == null)).AsQueryable();
            ViewBag.UsersProfiles = objNotesEntities.UserProfile.ToList();

            switch (SortOrder)
            {
                case "CreatedDate_asc":
                    admin = admin.OrderBy(x => x.ModifiedDate);
                    break;
                case "FirstName_desc":
                    admin = admin.OrderByDescending(x => x.FirstName);
                    break;
                case "FirstName":
                    admin = admin.OrderBy(x => x.FirstName);
                    break;
                case "LastName_desc":
                    admin = admin.OrderByDescending(x => x.LastName);
                    break;
                case "LastName":
                    admin = admin.OrderBy(x => x.LastName);
                    break;
                case "EmailID_desc":
                    admin = admin.OrderByDescending(x => x.EmailId);
                    break;
                case "EmailID":
                    admin = admin.OrderBy(x => x.EmailId);
                    break;
                case "Active_desc":
                    admin = admin.OrderByDescending(x => x.IsActive);
                    break;
                case "Active":
                    admin = admin.OrderBy(x => x.IsActive);
                    break;
                default:
                    admin = admin.OrderByDescending(x => x.ModifiedDate);
                    break;
            }

            return View(admin.ToList().ToPagedList(page ?? 1, 5));

        }

        [Route("AddAdministrator")]
        [HttpGet]
        public ActionResult AddAdministrator()
        {
            ViewBag.PhoneNumberCountryCode = objNotesEntities.Country.Distinct().Where(x => x.IsActive == true).ToList();
            return View();
        }

        [Route("AddAdministrator")]
        [HttpPost]
        public ActionResult AddAdministrator(AddAdministrator addadministrator)
        {
            if (ModelState.IsValid)
            {
                var Emailid = User.Identity.Name.ToString();
                Users userObj = objNotesEntities.Users.Where(x => x.EmailId == Emailid).FirstOrDefault();
                Users admin = new Users
                {
                    FirstName = addadministrator.FirstName,
                    LastName = addadministrator.LastName,
                    EmailId = addadministrator.EmailID,
                    RoleID = objNotesEntities.UserRole.Where(x => x.Name.ToLower() == "administrator").Select(y => y.ID).FirstOrDefault(),
                    Password = EncryptPasswords.EncryptPasswordMd5("Admin@123"),
                    CreatedBy = userObj.ID,
                    ModifiedBy = userObj.ID,
                    CreateDate = DateTime.Now,
                    ModifiedDate = DateTime.Now

                };
                objNotesEntities.Users.Add(admin);

             /*   string activationCode = Guid.NewGuid().ToString();
                admin.VerificationCode = activationCode;*/
               /* objNotesEntities.Users.Add(admin);*/
                objNotesEntities.SaveChanges();

                // Generating Email Verification Link
                /*var activationCode = obj.Password;*/
 /*               var verifyUrl = "/Account/VerifyAccount/" + activationCode;
                var activationlink = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
                var emailid = objNotesEntities.SystemConfigurations.Where(x => x.keys.ToLower() == "supportemailid").Select(x => x.Value).FirstOrDefault();
                var password = objNotesEntities.SystemConfigurations.Where(x => x.keys.ToLower() == "supportemailidpassword").Select(x => x.Value).FirstOrDefault();
                // Sending Email
                EmailVerificationLink.SendVerificationLinkEmail(admin, activationlink, emailid, password);
 */
                var id = admin.ID;

                UserProfile adminprofile = new UserProfile
                {
                    PhoneCountryCode = addadministrator.PhoneCountryCode,
                    PhoneNumber = addadministrator.PhoneNumber,
                    UserID = id,
                    /*                    ProfilePicture = objNotesEntities.SystemConfigurations.Where(x => x.keys == "DefaultUserPicture").Select(x => x.Value).FirstOrDefault(),
                    */
                    AddressLine1 = "x",
                    AddressLine2 = "x",
                    City = "x",
                    State = "x",
                    ZipCode = "x",
                    Country = 0,




                    
                   
                    CreatedBy = userObj.ID,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = userObj.ID,
                    ModifiedDate = DateTime.Now,
                    
                };

                objNotesEntities.UserProfile.Add(adminprofile);
                objNotesEntities.SaveChanges();

                string storepath = Path.Combine(Server.MapPath("~/Members/"), id.ToString());
                if (!Directory.Exists(storepath))
                {
                    Directory.CreateDirectory(storepath);
                }
                TempData["AddAdministrator"] = userObj.FirstName + " " + userObj.LastName;
                TempData["Message"] = "Administrator has been Successfully Added !";
                return RedirectToAction("AddAdministrator", "Settings");
            }
            return View();
        }

        [Route("EditAdministrator/{id}")]
        [HttpGet]
        public ActionResult EditAdministrator(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Users admin = objNotesEntities.Users.Find(id);
            UserProfile adminprofile = objNotesEntities.UserProfile.Where(x => x.UserID == admin.ID).FirstOrDefault();

            EditAdministrator addadministrator = new EditAdministrator
            {
                UserID = admin.ID,
                FirstName = admin.FirstName,
                LastName = admin.LastName,
                EmailID = admin.EmailId,
                PhoneNumber = adminprofile.PhoneNumber,
                IsEmailVerified = admin.IsEmailVerified
            };

            if (admin == null)
            {
                return RedirectToAction("Error", "HomePage");
            }

            ViewBag.PhoneNumberCountryCode = new SelectList(objNotesEntities.Country.Distinct().Where(x => x.IsActive == true), "CountryCode", "CountryCode", adminprofile.PhoneCountryCode);
            return View(addadministrator);
        }

        [Route("EditAdministrator/{id}")]
        [HttpPost]
        public ActionResult EditAdministrator(int? id, EditAdministrator addadministrator)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Users admin = objNotesEntities.Users.Find(id);
                UserProfile adminprofile = objNotesEntities.UserProfile.Where(x => x.UserID == admin.ID).FirstOrDefault();
                if (admin == null)
                {
                    return RedirectToAction("Error", "HomePage");
                }

                var Emailid = User.Identity.Name.ToString();
                Users userObj = objNotesEntities.Users.Where(x => x.EmailId == Emailid).FirstOrDefault();

                admin.FirstName = addadministrator.FirstName;
                admin.LastName = addadministrator.LastName;
                admin.EmailId = addadministrator.EmailID;
                admin.ModifiedBy = userObj.ID;
                admin.ModifiedDate = DateTime.Now;

                objNotesEntities.Entry(admin).State = EntityState.Modified;
                objNotesEntities.SaveChanges();

                adminprofile.PhoneCountryCode = addadministrator.PhoneNumberCountryCode;
                adminprofile.PhoneNumber = addadministrator.PhoneNumber;

                objNotesEntities.Entry(adminprofile).State = EntityState.Modified;
                objNotesEntities.SaveChanges();

                TempData["AddAdministrator"] = userObj.FirstName + " " + userObj.LastName;
                TempData["Message"] = "Administrator has been Successfully Edited !";
                return RedirectToAction("Administrators", "Admin");
            }
            return View();
        }

        [Route("DeleteAdministrator/{id}")]
        [HttpGet]
        public ActionResult DeleteAdministrator(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users admin = objNotesEntities.Users.Find(id);

            if (admin == null)
            {
                return RedirectToAction("Error", "HomePage");
            }
            admin.IsActive = false;

            objNotesEntities.Entry(admin).State = EntityState.Modified;
            objNotesEntities.SaveChanges();

          
            var Emailid = User.Identity.Name.ToString();
            Users userObj = objNotesEntities.Users.Where(x => x.EmailId == Emailid).FirstOrDefault();
            TempData["AddAdministrator"] = userObj.FirstName + " " + userObj.LastName;
            TempData["Message"] = "Administrator has been Successfully Deleted !";
            return RedirectToAction("Administrators", "Admin");

        }











        public ActionResult AddCategory()
        {
            return View();
        }
        public ActionResult ManageCategory()
        {
            return View();
        }

        public ActionResult AddType()
        {
            return View();
        }
        public ActionResult ManageType()
        {
            return View();
        }

        public ActionResult AddCountry()
        {
            return View();
        }
        public ActionResult ManageCountries()
        {
            return View();
        }

    }
}