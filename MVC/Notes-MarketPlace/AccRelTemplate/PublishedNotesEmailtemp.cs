using Notes_MarketPlace.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Notes_MarketPlace.AccRelTemplate
{
    public class PublishedNotesEmailtemp
    {
        public static void PublishedNoteNotify(Users sellerDetail, string notetitle)
        {
            var fromEmail = new MailAddress("arpitprajapati92.am@gmail.com", "NotesMarketPlace"); //need system email
            var toEmail = new MailAddress("arpitprajapati92.am@gmail.com");
            var fromEmailPassword = "1003@Appu"; // Replace with actual password
            string subject = " " + sellerDetail.FirstName + " " + sellerDetail.LastName + " - sent his note for review";
            string body = "Hello Admin,<br/> We want to inform you that,<b>" + sellerDetail.FirstName + " " + sellerDetail.LastName + "</b>" +
                " sent his note <b>" + notetitle + "</b>" + " for review. Please look at the notes and take required actions. <br/>";
            body += "<br/><br/>Regards,<br/>";
            body += "NotesMarketPlace";


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
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }


    }
}