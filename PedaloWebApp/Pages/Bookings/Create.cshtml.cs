namespace PedaloWebApp.Pages.Bookings
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.FileSystemGlobbing.Internal;
    using PedaloWebApp.Core.Domain.Entities;
    using PedaloWebApp.Core.Interfaces.Data;

    public class CreateModel : PageModel
    {
        private readonly IDbContextFactory contextFactory;

        public CreateModel(IDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        [BindProperty]
        public BookingCreateModel Booking { get; set; }

        [BindProperty]
        public List<Pedalo> Pedalos { get; set; }

        [BindProperty]
        public List<Customer> Customer { get; set; }

        [BindProperty]
        public List<Passenger> Passenger { get; set; }

        [BindProperty]
        public PassengerCreateModel Passengers { get; set; }


        public IActionResult OnGet()
        {
            using var context = this.contextFactory.CreateContext();
            this.Pedalos = context.Pedaloes.OrderBy(x => x.Name).ThenBy(x => x.Color).ToList();
            this.Customer = context.Customers.OrderBy(x => x.FirstName).ThenBy(x => x.LastName).ToList();
            this.Passenger = context.Passengers.OrderBy(x => x.Firstname).ThenBy(x => x.Lastname).ToList();
            return this.Page();
        }

        public IActionResult OnPost()
        {
            if (!this.ModelState.IsValid)
            {
                //return this.Page();
                
            }

            using var context = this.contextFactory.CreateContext();

            try
            {
                /*DateTime dateTime = this.Booking.StartDate;
                DateTime newDateTime = DateTime.ParseExact(dateTime.ToString("yyyy/MM/dd"), "yyyy/MM/dd", null);*/

                var booking = new Booking { 
                    CustomerId = this.Booking.CustomerId,
                    PedaloId = this.Booking.PedaloId,
                    StartDate = this.Booking.StartDate,
                    EndDate = this.Booking.EndDate
                };
                context.Bookings.Add(booking);
                context.SaveChanges();
                var pedalo = context.Pedaloes.Where(x => x.PedaloId == booking.PedaloId).Single();

                if(pedalo.Capacity != 1)
                {
                    return this.RedirectToPage("AddPassenger", new { bookingid = booking.BookingId, capacity = pedalo.Capacity });
                }
                else
                {
                    return this.RedirectToPage("Bookings/Index");

                }

            }
            catch (Exception)
            {
                return this.RedirectToPage("/Error");
            }

        }
    }

    public class BookingCreateModel
    {
        public Guid BookingId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid PedaloId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Pedalo Pedalo { get; set; }
        public Customer Customer { get; set; }
    }

    public class PassengerCreateModel
    {
        public Guid PassengerId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

    }
}

