namespace PedaloWebApp.Pages.Bookings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.EntityFrameworkCore;
    using PedaloWebApp.Core.Domain.Entities;
    using PedaloWebApp.Core.Interfaces.Data;
    using QuestPDF.Fluent;
    using QuestPDF.Helpers;
    using QuestPDF.Infrastructure;
    using MailKit.Net.Smtp;
    using MimeKit;

    public class IndexModel : PageModel
    {
        private readonly IDbContextFactory contextFactory;

        public IndexModel(IDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public IReadOnlyList<Booking> Bookings { get; set; }
        public IReadOnlyList<Pedalo> Pedalos { get; set; }
        public IReadOnlyList<Passenger> Passengers { get; set; }


        public int Loadingcolumns { get; set; }

        public IActionResult OnGet()
        {
            using var context = this.contextFactory.CreateReadOnlyContext();
            this.Bookings = context.Bookings
                .Include(x => x.Customer)
                .Include(x => x.Pedalo)
                .ToList();
            this.Pedalos = context.Pedaloes.ToList();
            this.Passengers = context.Passengers.ToList();

            if (Bookings.Count > 10)
            {
                Loadingcolumns = 10;
            }
            else
            {
                Loadingcolumns = Bookings.Count;
            }

            // load the passengers for each booking
            foreach (var booking in this.Bookings)
            {
                booking.BookingPassengers = context.BookingPassengers
                .Where(x => x.BookingId == booking.BookingId)
                .ToList();
            }

            return this.Page();
        }


        
        public void OnPostLoadmore()
        {
            using var context = this.contextFactory.CreateReadOnlyContext();
            this.Bookings = context.Bookings
                .Include(x => x.Customer)
                .Include(x => x.Pedalo)
                .ToList();
            this.Pedalos = context.Pedaloes.ToList();
            this.Passengers = context.Passengers.ToList();

            
            // load the passengers for each booking
            foreach (var booking in this.Bookings)
            {
                booking.BookingPassengers = context.BookingPassengers
                .Where(x => x.BookingId == booking.BookingId)
                .ToList();
            }
        
            Loadingcolumns = this.Bookings.Count;
        }

        //public async Task<IActionResult> OnPostSendEmailAsync()
        //{
        //    using var context = this.contextFactory.CreateReadOnlyContext();
        //    this.Bookings = context.Bookings
        //        .Include(x => x.Customer)
        //        .Include(x => x.Pedalo)
        //        .ToList();
        //    this.Pedalos = context.Pedaloes.ToList();
        //    this.Passengers = context.Passengers.ToList();
        //
        //    if (Bookings.Count > 10)
        //    {
        //        Loadingcolumns = 10;
        //    }
        //    else
        //    {
        //        Loadingcolumns = Bookings.Count;
        //    }
        //
        //    // load the passengers for each booking
        //    foreach (var booking in this.Bookings)
        //    {
        //        booking.BookingPassengers = context.BookingPassengers
        //        .Where(x => x.BookingId == booking.BookingId)
        //        .ToList();
        //
        //    }
        //
        //    // Create a new MimeMessage
        //    var message = new MimeMessage();
        //
        //
        //
        //    // Set the "from" address
        //    message.From.Add(new MailboxAddress("Kaan", "20kaan05@gmail.com"));
        //
        //
        //
        //    // Set the "to" address
        //    message.To.Add(new MailboxAddress("Kaan", "20kaan05@gmail.com"));
        //
        //
        //
        //    // Set the subject of the email
        //    message.Subject = "Booking confirmation";
        //
        //
        //
        //    // Set the body of the email
        //    message.Body = new TextPart("plain")
        //    {
        //        Text = "Booking ID:" + Bookings + "\nCustomer: " + this.Bookings.Customer.FirstName + "\nPedalo: " + this.Bookings.Pedalo.Name + "\nStart date: " + this.Bookings.StartDate + "\nEnd date:" + this.Bookings.EndDate
        //        //Text = "Buchungsbestätigung"
        //    };
        //
        //
        //
        //    // Connect to the SMTP server
        //    using (var client = new MailKit.Net.Smtp.SmtpClient())
        //    {
        //        // Replace "smtp.example.com" with the hostname of your SMTP server
        //        await client.ConnectAsync("smtp.gmail.com", 587, false);
        //
        //
        //        // Replace "your.email@example.com" and "your-password" with your actual email address and password
        //        await client.AuthenticateAsync("20kaan05@gmail.com", "cozqcaueuhuthuic"); //Email (App passwort)
        //
        //
        //        // Send the email
        //        await client.SendAsync(message);
        //
        //
        //        // Disconnect from the SMTP server
        //        await client.DisconnectAsync(true);
        //    }
        //
        //    // Redirect to a success page
        //    return this.Page();
        //}

    }
}


