namespace PedaloWebApp.Pages.Bookings
{
    using PedaloWebApp.Pages.Passengers;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using PedaloWebApp.Core.Domain.Entities;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.FileSystemGlobbing.Internal;
    using PedaloWebApp.Core.Interfaces.Data;
    using MimeKit;
    using System.Threading.Tasks;

    public class CreatePassengerModel : PageModel
    {
        private readonly IDbContextFactory contextFactory;
        public CreatePassengerModel(IDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        [BindProperty]
        public List<Booking> Bookings { get; set; }

        [BindProperty]
        public List<Passenger> Passenger { get; set; }

        [BindProperty]
        public List<Customer> Customers { get; set; }

        [FromQuery(Name = "bookingid")]
        public Guid BookingId { get; set; }

        [BindProperty]
        public Guid[] PassengerIds { get; set; }

        //[FromQuery(Name = "capacity")]
        public int Capacity { get; set; }
        public string CustomerName { get; set; }
        public IActionResult OnGet()
        {
            using var context = this.contextFactory.CreateContext();
            this.Passenger = context.Passengers.ToList().OrderBy(x => x.Firstname).ThenBy(x => x.Lastname).ToList();

            var booking = context.Bookings.FirstOrDefault(x => x.BookingId == this.BookingId);
            var pedalo = context.Pedaloes.FirstOrDefault(x => x.PedaloId == booking.PedaloId);
            var capacity = pedalo.Capacity;
            Capacity = capacity;

            var customer = context.Customers.FirstOrDefault(x => x.CustomerId == booking.CustomerId);
            var Customername = customer.FirstName;
            CustomerName = Customername;

            //var passenger = context.Passengers.Where(x => x.PassengerId == );
            //var selectedPassenger = customer.LastName;

            this.PassengerIds = new Guid[capacity];

            var existingBookingPassenger = context.BookingPassengers.Where(x => x.BookingId == this.BookingId).OrderBy(x => x.Passenger.Firstname).ThenBy(p => p.Passenger.Lastname).ToList();
            int i = 0;
            foreach (var passenger in existingBookingPassenger)
            {
                this.PassengerIds[i] = passenger.PassengerId;
                i++;
            }

            //this.PassengerIds.OrderBy(x => );
            //this.PassengerIds = context.Passengers.OrderBy(x => x.Firstname);

            return this.Page();
        }

        public void OnPost()
        {

            using var context = this.contextFactory.CreateContext();
            var existingBookingPassenger = context.BookingPassengers.Where(x => x.BookingId == this.BookingId).ToList();
            existingBookingPassenger.OrderBy(x => x.Passenger.Firstname);
            foreach (var passenger in existingBookingPassenger)
            {
                context.BookingPassengers.Remove(passenger);
            }

            context.SaveChanges();

            var passengerids = PassengerIds.Where(x => x != Guid.Empty).ToList();
            foreach (var item in passengerids)
            {
                var passengerbooking = new BookingPassenger
                {
                    BookingId = this.BookingId,
                    PassengerId = item,
                };
                context.BookingPassengers.Add(passengerbooking);
            }


            context.SaveChanges();

            SendEmailAsync();
        }

        public async Task<IActionResult> SendEmailAsync()
        {


            // Create a new MimeMessage
            var message = new MimeMessage();



            // Set the "from" address
            message.From.Add(new MailboxAddress("Kaan", "20kaan05@gmail.com"));



            // Set the "to" address
            message.To.Add(new MailboxAddress("Kaan", "20kaan05@gmail.com"));



            // Set the subject of the email
            message.Subject = "Booking confirmation";



            // Set the body of the email
            message.Body = new TextPart("plain")
            {
                Text = "Booking ID:" + Bookings[0].BookingId + "\nCustomer: " + Bookings[0].Customer.FirstName + "\nPedalo: " + Bookings[0].Pedalo.Name + "\nStart date: " + Bookings[0].StartDate + "\nEnd date:" + Bookings[0].EndDate

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
            return this.RedirectToPage("./Index");
        }
    }
}

