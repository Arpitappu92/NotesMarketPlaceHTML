using Notes_MarketPlace.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Notes_MarketPlace.AccRelTemplate
{
    public class ForgetPasswordEmail
    {
        public static void SendOtpToEmail(Users objUser, int otp)
        {
            var fromEmail = new MailAddress("arpitprajapati92.am@gmail.com", "Notes Marketplace"); //need system email
            var toEmail = new MailAddress(objUser.EmailId);
            var fromEmailPassword = "2012@Appu"; // Replace with actual password
            string subject = "New Temporary Password has been created for you";
            string msg = "Hello " + objUser.FirstName + " " + objUser.LastName + "<br/>";
            msg += "We have generated a new password for you <br/>";
            msg += "Password: " + otp;
            msg += "<br/><br/>Regards,<br/>";
            msg += "Notes Marketplace";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = msg,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }
    }
}