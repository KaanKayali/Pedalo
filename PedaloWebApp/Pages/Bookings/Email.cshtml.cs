namespace PedaloWebApp.Pages.Bookings
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using System.Net.Mail;
    using System;
    using System.Net;
    using System.Net.Mail;
    using System.Net.Mime;

    public class EmailModel : PageModel
    {
        
        public void OnGet()
        {
            Email();
        }

        public void Email() 
        {
            try
            {

                SmtpClient mySmtpClient = new SmtpClient("my.smtp.exampleserver.net");

                // set smtp-client with basicAuthentication
                mySmtpClient.UseDefaultCredentials = false;
                System.Net.NetworkCredential basicAuthenticationInfo = new System.Net.NetworkCredential("username", "password");
                mySmtpClient.Credentials = basicAuthenticationInfo;

                // add from,to mailaddresses
                MailAddress from = new MailAddress("20kaan05@gmail.com", "Kaan");
                MailAddress to = new MailAddress("20kaan05@gmail.com", "Kaan");
                MailMessage myMail = new System.Net.Mail.MailMessage(from, to);

                // add ReplyTo
                MailAddress replyTo = new MailAddress("20kaan05@gmail.com");
                myMail.ReplyToList.Add(replyTo);

                // set subject and encoding
                myMail.Subject = "Mcdonalds burger";
                myMail.SubjectEncoding = System.Text.Encoding.UTF8;

                // set body-message and encoding
                myMail.Body = "<b>Test Mail</b><br>using <b>HTML</b>.";
                myMail.BodyEncoding = System.Text.Encoding.UTF8;
                // text or html
                myMail.IsBodyHtml = true;

                mySmtpClient.Send(myMail);
            }

            catch (SmtpException ex)
            {
                throw new ApplicationException
                  ("SmtpException has occured: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}