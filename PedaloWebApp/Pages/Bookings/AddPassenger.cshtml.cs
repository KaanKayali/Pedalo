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
        public Guid[] PassangerId { get; set; }

        //[FromQuery(Name = "capacity")]
        public int Capacity { get; set; }
        public string CustomerName { get; set; }
        public string selectedPassenger { get; set; }
        public IActionResult OnGet()
        {
            using var context = this.contextFactory.CreateContext();
            this.Passenger = context.Passengers.ToList();

            var booking = context.Bookings.FirstOrDefault(x => x.BookingId == this.BookingId);
            var pedalo = context.Pedaloes.FirstOrDefault(x => x.PedaloId == booking.PedaloId);
            var capacity = pedalo.Capacity;
            Capacity = capacity;

            var customer = context.Customers.FirstOrDefault(x => x.CustomerId == booking.CustomerId);
            var Customername = customer.FirstName;
            CustomerName = Customername;

            //var passenger = context.Passengers.Where(x => x.PassengerId == );
            //var selectedPassenger = customer.LastName;

            this.PassangerId = context.BookingPassengers.Where(x => x.BookingId == this.BookingId).Select(x => x.PassengerId).ToArray();
            return this.Page();
        }

        public IActionResult OnPost()
        {
            
            using var context = this.contextFactory.CreateContext();
            var existingBookingPassenger = context.BookingPassengers.Where(x => x.BookingId == this.BookingId).ToList();

            foreach (var passenger in existingBookingPassenger)
            {
                context.BookingPassengers.Remove(passenger);
            }

            context.SaveChanges();

            foreach (var item in PassangerId) { 
                var passengerbooking = new BookingPassenger
                {
                    BookingId = this.BookingId,
                    PassengerId = item,
                };
                context.BookingPassengers.Add(passengerbooking);
            }
            

            context.SaveChanges();
           

            return this.RedirectToPage("./Index");
        }
    }
}
