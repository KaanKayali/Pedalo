using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PedaloWebApp.Core.Interfaces.Data;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Linq;

namespace PedaloWebApp.Pages.Bookings
{
    public class DownloadPDFModel : PageModel
    {
        private readonly IDbContextFactory contextFactory;

        public DownloadPDFModel(IDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }
        public IActionResult OnGet([FromQuery] Guid bookingId)
        {
            using var context = this.contextFactory.CreateReadOnlyContext();

            var booking = context.Bookings
                .Where(m => m.BookingId == bookingId)
                .Include(x => x.BookingPassengers).ThenInclude(x => x.Passenger)
                .Include(x => x.Customer)
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
                            x.Item().Text("Name: " + booking.Customer.FirstName + " " + booking.Customer.LastName + "\n").FontSize(14);
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
                            foreach (var item in booking.BookingPassengers)
                            {
                                x.Item().Text(item.Passenger.Firstname + " " + item.Passenger.Lastname + "\n").FontSize(14);
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
