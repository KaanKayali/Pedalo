namespace PedaloWebApp.Pages.Bookings
{
    using PedaloWebApp.Pages.Passengers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using PedaloWebApp.Core.Domain.Entities;
    using PedaloWebApp.Core.Interfaces.Data;

    public class EditModel : PageModel
    {
        private readonly IDbContextFactory contextFactory;

        public EditModel(IDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        [BindProperty]
        public BookingEditModel Booking { get; set; }

        [BindProperty]
        public List<Pedalo> Pedalos { get; set; }

        public List<BookingPassengerAmount> Passengos { get; set; }

        //[BindProperty]
        //public List<Passenger> Passengers { get; set; }
        
        //[FromQuery(Name = "bookingid")]
        //public Guid BookingId { get; set; }

        [BindProperty]
        public Guid[] PassangerId { get; set; }

        [BindProperty]
        public List<Customer> Customer { get; set; }

        public int Capacity { get; set; }
        public string AmountPassengers { get; set; }

        public IActionResult OnGet(Guid? id)
        {
            if (id == null)
            {
                return this.BadRequest();
            }

            using var context = this.contextFactory.CreateReadOnlyContext();
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

            //var booking = context.Bookings.FirstOrDefault(x => x.BookingId == id);
            var bookingpassenger = context.BookingPassengers.FirstOrDefault(x => x.BookingId == id);
            var passenger = context.BookingPassengers.FirstOrDefault(x => x.PassengerId == bookingpassenger.PassengerId);
            var amountpassenger = context.Passengers.FirstOrDefault(x => x.PassengerId == passenger.PassengerId);
            //AmountPassengers = amountpassenger.Firstname;

            this.Passengos = context.BookingPassengers.Select(x => new BookingPassengerAmount
            {
                BookingId = x.BookingId,
                PassengerName = amountpassenger.Firstname,
                TotalPassengers = context.Passengers.Count()
            }).ToList();

            this.Pedalos = context.Pedaloes.ToList();
            this.Customer = context.Customers.ToList();

            if (this.Booking == null)
            {
                return this.NotFound();
            }

            return this.Page();

        }

        public IActionResult OnPost()
        {
            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            using var context = this.contextFactory.CreateContext();
            var booking = context.Bookings.FirstOrDefault(x => x.BookingId == this.Booking.BookingId);
            if (booking == null)
            {
                return this.NotFound();
            }

            try
            {
                booking.CustomerId = this.Booking.CustomerId;
                booking.PedaloId = this.Booking.PedaloId;
                booking.StartDate = this.Booking.StartDate;
                booking.EndDate = this.Booking.EndDate;

                context.SaveChanges();
            }
            catch (Exception)
            {
                return this.RedirectToPage("/Error");
            }

            return this.RedirectToPage("./Index");
        }
    }

    public class BookingEditModel
    {
        public Guid BookingId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid PedaloId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Pedalo Pedalo { get; set; }
        public Customer Customer { get; set; }
    }
}
