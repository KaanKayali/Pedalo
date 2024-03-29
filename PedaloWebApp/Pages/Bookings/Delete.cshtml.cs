namespace PedaloWebApp.Pages.Bookings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using PedaloWebApp.Core.Domain.Entities;
    using PedaloWebApp.Core.Interfaces.Data;
    using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

    public class DeleteModel : PageModel
    {
        private readonly IDbContextFactory contextFactory;

        public DeleteModel(IDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        [BindProperty]
        public BookingDeleteModel Booking { get; set; }

        public IActionResult OnGet(Guid? id)
        {
            if (id == null)
            {
                return this.BadRequest();
            }

            using var context = this.contextFactory.CreateReadOnlyContext();
            this.Booking = context.Bookings
                .Where(m => m.BookingId == id)
                .Select(x => new BookingDeleteModel
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

            //context.BookingPassengers.RemoveRange(booking.BookingPassengers);
            //context.BookingPassengers.
            //context.CommandText = $"ALTER TABLE {BookingPassengers} DROP CONSTRAINT {constraintName}";
            //await context.ExecuteNonQueryAsync();
            //var bookingPassengers = await context.BookingPassengers.Where(bp => bp.BookingId == id).ToListAsync(); 
            context.BookingPassengers.RemoveRange(context.BookingPassengers.Where(x => x.BookingId == this.Booking.BookingId));
            context.Bookings.Remove(booking);
            //var booking = context.Bookings.FirstOrDefault(x => x.BookingId == this.BookingId);
            //var pedalo = context.Pedaloes.FirstOrDefault(x => x.PedaloId == booking.PedaloId);
            //var capacity = pedalo.Capacity;

                context.SaveChanges();
            }
            catch (Exception)
            {
                return this.RedirectToPage("/Error");
            }

            return this.RedirectToPage("./Index");
        }
    }

    public class BookingDeleteModel
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
