﻿namespace PedaloWebApp.Pages.Bookings
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.EntityFrameworkCore;
    using PedaloWebApp.Core.Domain.Entities;
    using PedaloWebApp.Core.Interfaces.Data;

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

    }
}


