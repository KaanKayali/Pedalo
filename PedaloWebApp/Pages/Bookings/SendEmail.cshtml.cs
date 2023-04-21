namespace PedaloWebApp.Pages.Bookings
{
    using DocumentFormat.OpenXml.Office2010.Excel;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using MimeKit;
    using PedaloWebApp.Core.Domain.Entities;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Linq;
    using PedaloWebApp.Core.Interfaces.Data;
    public class SendEmailModel : PageModel
    {
        private readonly IDbContextFactory contextFactory;

        public SendEmailModel(IDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        [BindProperty]
        public BookingEditModel Booking { get; set; }

        [FromQuery(Name = "bookingid")]
        public Guid BookingId { get; set; }

        [BindProperty]
        public List<Customer> Customer { get; set; }

        public int Capacity { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {

            using var context = this.contextFactory.CreateReadOnlyContext();
            this.Booking = context.Bookings
            .Where(m => m.BookingId == this.BookingId)
            .Select(x => new BookingEditModel
            {
                BookingId = x.BookingId,
                CustomerId = x.CustomerId,
                PedaloId = x.PedaloId,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Pedalo = x.Pedalo,
                Customer = x.Customer,
            })
            .FirstOrDefault();

            if (this.Booking == null)
            {
                this.Booking = context.Bookings
                .Where(m => m.BookingId == id)
                .Select(x => new BookingEditModel
                {
                    BookingId = x.BookingId,
                    CustomerId = x.CustomerId,
                    PedaloId = x.PedaloId,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Pedalo = x.Pedalo,
                    Customer = x.Customer,
                })
                .FirstOrDefault();
            }
            

            // Create a new MimeMessage
            var message = new MimeMessage();


            message.From.Add(new MailboxAddress("Kaan", "20kaan05@gmail.com"));

            
            message.To.Add(new MailboxAddress(Booking.Customer.FirstName, "20kaan05@gmail.com")); //Booking.Customer.Email

            message.Subject = "Booking confirmation";


            // Set the body of the email
            message.Body = new TextPart("plain")
            {
                Text = "Booking ID:" + Booking.BookingId + "\nCustomer: " + Booking.Customer.FirstName + " " + Booking.Customer.LastName + "\nPedalo: " + Booking.Pedalo.Name + "\nStart date: " + Booking.StartDate + "\nEnd date:" + Booking.EndDate

            };



            // Connect to the SMTP server
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                // Replace "smtp.example.com" with the hostname of your SMTP server
                await client.ConnectAsync("smtp.gmail.com", 587, false);


                // Replace "your.email@example.com" and "your-password" with your actual email address and password
                await client.AuthenticateAsync("20kaan05@gmail.com", "cozqcaueuhuthuic"); //Email (App passwort)


                // Send the email
                await client.SendAsync(message);


                // Disconnect from the SMTP server
                await client.DisconnectAsync(true);
            }

            // Redirect to a success page
            return this.RedirectToPage("Index");
        }
    }
}
