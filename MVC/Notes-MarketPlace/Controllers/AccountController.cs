using Notes_MarketPlace.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using Notes_MarketPlace.Helpers;
using System.Web.Security;
using Notes_MarketPlace.AccRelTemplate;

namespace Notes_MarketPlace.Controllers
{
    [RoutePrefix("Account")]
    public class AccountController : Controller
    {
        private NotesEntities objNotesEntities = new NotesEntities();

        [Route("Register")]
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        
        [Route("Register")]
        [HttpPost]
        public ActionResult Register(RegistrationModel objM)
        {
            if(ModelState.IsValid)
            {
                Users obj = new Users
                {
                    FirstName = objM.FirstName,
                    LastName = objM.LastName,
                    EmailId = objM.EmailId,
                    RoleID = 3,
                    IsActive = true,
                    CreateDate = DateTime.Now,
                    IsEmailVerified = false
                };
                obj.Password = EncryptPasswords.EncryptPasswordMd5(objM.Password);

                objNotesEntities.Users.Add(obj);
                objNotesEntities.SaveChanges();


                var activationCode = obj.Password;
                var verifyUrl = "/Account/VerifyAccount/" + activationCode;
                var activationlink = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

                EmailVerificationTemp.SendVerifyLinkEmail(obj, activationlink);
                ViewBag.Title = "Notes_MarketPlace";
                @TempData["UserName"] = obj.FirstName.ToString();
                return new RedirectResult(@"~/Account/EmailVerification");

            }
            return View();
        }


        [Route("EmailVerification")]
        public ActionResult EmailVerification()
        {
            return View();
        }

        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            using (NotesEntities DBobj = new NotesEntities())
            {
                DBobj.Configuration.ValidateOnSaveEnabled = false; 
                var ema = DBobj.Users.Where(x => x.Password == id).FirstOrDefault();
                if (ema != null)
                {
                    ema.IsEmailVerified = true;
                    DBobj.SaveChanges();
                }
                else
                {
                    ViewBag.Message = "Invalid Request";
                }
            }

            TempData["Message"] = "Your Email Is Verified You Can Login Here";
            return RedirectToAction("Login", "Account");
        }

        public JsonResult EmailExist(string EmailId)
        {
            return Json(!objNotesEntities.Users.Any(u => u.EmailId == EmailId), JsonRequestBehavior.AllowGet);
        }





        [Route("Login")]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [Route("Login")]
        [HttpPost]
        public ActionResult Login(LoginModel ObjLogin)
        {
            if (ModelState.IsValid)
            {

                var newPassword = EncryptPasswords.EncryptPasswordMd5(ObjLogin.Password);

                bool isValid = objNotesEntities.Users.Any(x => x.EmailId == ObjLogin.EmailId && x.Password == newPassword);
                if (isValid)
                {
                    Users userDetails = objNotesEntities.Users.Where(x => x.EmailId == ObjLogin.EmailId && x.Password == newPassword).FirstOrDefault();
                    if (userDetails.IsEmailVerified)
                    {
                        HttpCookie ic = new HttpCookie("ID", userDetails.ID.ToString());
                        Response.Cookies.Add(ic);
                          
                        
                        FormsAuthentication.SetAuthCookie(ObjLogin.EmailId, ObjLogin.RememberMe);




                        if (userDetails.RoleID == objNotesEntities.UserRole.Where(x => x.Name.ToLower() == "administrator").Select(x => x.ID).FirstOrDefault())
                        {
                            return RedirectToAction("Dashboard", "Admin");
                        }
                        else
                        {

                  
                            return RedirectToAction("Dashboard", "Front");
                        }

                    }
                    TempData["Error"] = "Email Address Is Not Verified";
                    return View();
                }
                TempData["Error"] = "Invalid username or password";
                return View();
            }

            return View();
        }



        [Route("ForgetPassword")]
        [HttpGet]
        public ActionResult ForgetPassword()
        {
            return View();
        }
        [Route("ForgetPassword")]
        [HttpPost]
        public ActionResult ForgetPassword(ForgetPasswordModel obj)
        {
            if (ModelState.IsValid)
            {
                bool isValid = objNotesEntities.Users.Any(x => x.EmailId == obj.EmailId);
                if (isValid)
                {
                    Users userDetails = objNotesEntities.Users.Where(x => x.EmailId == obj.EmailId).FirstOrDefault();
                    Random rand = new Random();
                    var otp = rand.Next();
                    var strotp = otp.ToString();

                    userDetails.Password = EncryptPasswords.EncryptPasswordMd5(strotp);
                    objNotesEntities.SaveChanges();

                    ForgetPasswordEmail.SendOtpToEmail(userDetails, otp);

                    TempData["Message"] = "Otp Sent To Your Registered EmailAddress use it for login";
                    return RedirectToAction("Login", "Account");

                }
                TempData["Error"] = "Invalid EmailAddress";
                return View();
            }
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }


    }
}
