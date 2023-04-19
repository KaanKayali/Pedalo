namespace PedaloWebApp.Pages.Bookings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.EntityFrameworkCore;
    using PedaloWebApp.Core.Domain.Entities;
    using PedaloWebApp.Core.Interfaces.Data;
    using QuestPDF.Fluent;
    using QuestPDF.Helpers;
    using QuestPDF.Infrastructure;

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

        [BindProperty]
        public BookingEditModel Booking { get; set; }

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

        public ActionResult OnPostDownloadPDF()
        {
            using var context = this.contextFactory.CreateReadOnlyContext();
            
            this.Pedalos = context.Pedaloes.ToList();
            this.Passengers = context.Passengers.ToList();
            this.Bookings = context.Bookings.ToList();

            //var booking = context.Bookings.FirstOrDefault(x => x.BookingId == this.Booking.BookingId);
            
            this.Booking = context.Bookings
                .Where(m => m.BookingId == this.Booking.BookingId)
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

            // code in your main method
            var pdfBytes =  Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(25));

                    page.Header()
                        .Text("Booking")
                        .FontSize(36);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            x.Item().BorderTop(1);
                            x.Item().Text("Pedalo:").FontSize(20).FontFamily("Segoe UI");
                            x.Item().Text("Customer:").FontSize(16);
                            //x.Item().Text("Name: " + booking.Customer.FirstName + " " + booking.Customer.LastName + "\n").FontSize(14);
                            //
                            //x.Item().Text("Pedalo:").FontSize(16);
                            //x.Item().Text("Name: " + booking.Pedalo.Name + "\n").FontSize(14);
                            //x.Item().Text("Color: " + booking.Pedalo.Color + "\n").FontSize(14);
                            //x.Item().Text("Capacity: " + booking.Pedalo.Capacity + "\n").FontSize(14);
                            //x.Item().Text("Hourly rate: " + booking.Pedalo.HourlyRate + "\n").FontSize(14);
                            //
                            //x.Item().Text("Dates:").FontSize(16);
                            //x.Item().Text("Start date: " + booking.StartDate + "\n").FontSize(14);
                            //x.Item().Text("End date: " + booking.EndDate + "\n").FontSize(14);

                            x.Item().Text("Passengers:").FontSize(16);
                            foreach(var item in Passengers)
                            {
                                x.Item().Text(item.Firstname + " " + item.Lastname + "\n").FontSize(14);
                            }
                            x.Item().BorderTop(1);
                        });

                });
            })
            .GeneratePdf();

            return File(pdfBytes, "application/pdf");
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
}


