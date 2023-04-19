namespace PedaloWebApp.Pages.Bookings
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using PedaloWebApp.Core.Domain.Entities;
    using System.Collections.Generic;
    using System;
    using QuestPDF.Fluent;
    using QuestPDF.Helpers;
    using QuestPDF.Infrastructure;

    public class GeneratePDFModel : PageModel
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

        public ActionResult OnGet()
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
            var pdfBytes = Document.Create(container =>
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
                            x.Item().Text("Name: " + this.Booking.Customer.FirstName + " " + this.Booking.Customer.LastName + "\n").FontSize(14);
                            
                            x.Item().Text("Pedalo:").FontSize(16);
                            x.Item().Text("Name: " + this.Booking.Pedalo.Name + "\n").FontSize(14);
                            x.Item().Text("Color: " + this.Booking.Pedalo.Color + "\n").FontSize(14);
                            x.Item().Text("Capacity: " + this.Booking.Pedalo.Capacity + "\n").FontSize(14);
                            x.Item().Text("Hourly rate: " + this.Booking.Pedalo.HourlyRate + "\n").FontSize(14);
                            
                            x.Item().Text("Dates:").FontSize(16);
                            x.Item().Text("Start date: " + this.Booking.StartDate + "\n").FontSize(14);
                            x.Item().Text("End date: " + this.Booking.EndDate + "\n").FontSize(14);

                            x.Item().Text("Passengers:").FontSize(16);
                            foreach (var item in Passengers)
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
    }
}
