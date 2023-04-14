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
        public List<Booking> Booking { get; set; }

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
        public IActionResult OnGet()
        {
            using var context = this.contextFactory.CreateContext();
            this.Passenger = context.Passengers.ToList();

            var bookingPassenger = context.BookingPassengers.FirstOrDefault(x => x.BookingId == this.BookingId);
            bookingPassenger.BookingPassengerId = context.BookingPassengers
                .Where(x => x.PassengerId == this.PassangerId[])
                .ToList();
            var booking = context.Bookings.FirstOrDefault(x => x.BookingId == this.BookingId);
            booking.Customer = context.Bookings
                .Where(x => x.CustomerId == this. )
                .ToList();
            return this.Page();
        }

        public IActionResult OnPost()
        {
            

            using var context = this.contextFactory.CreateContext();

        
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

        public class BookingPassengerCreateModel
        {
            public Guid BookingPassengerId { get; set; }
            public Guid BookingId { get; set; }
            public Guid PassengerId { get; set; }
            public int Capacity { get; set; }
            public Booking Booking { get; set; }
            public Passenger Passenger { get; set; }
        }
    }
}
