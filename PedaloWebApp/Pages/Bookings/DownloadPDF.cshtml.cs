namespace PedaloWebApp.Pages.Bookings
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.EntityFrameworkCore;
    using PedaloWebApp.Core.Interfaces.Data;
    using QuestPDF.Fluent;
    using QuestPDF.Helpers;
    using QuestPDF.Infrastructure;
    using System;
    using System.Drawing;
    using System.Linq;
    public class DownloadPDFModel : PageModel
    {
        private readonly IDbContextFactory contextFactory;

        public DownloadPDFModel(IDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }
        public string today_string = DateTime.Now.ToString("yyyy-MM-dd");
        public IActionResult OnGet([FromQuery] Guid bookingId)
        {
            using var context = this.contextFactory.CreateReadOnlyContext();

            var booking = context.Bookings
                .Where(m => m.BookingId == bookingId)
                .Include(x => x.BookingPassengers).ThenInclude(x => x.Passenger)
                .Include(x => x.Customer)
                .Include(x => x.Pedalo)
                .FirstOrDefault();

            int ColorValue = Color.FromName($"{booking.Pedalo.Color}").ToArgb();
            string ColorHex = string.Format("{0:x6}", ColorValue);


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
                        .Text("Booking " + today_string)
                        .FontSize(30);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            x.Item().BorderTop(1);
                            x.Item().Text("Customer:").FontSize(16);
                            x.Item().Text("Name: " + booking.Customer.FirstName + " " + booking.Customer.LastName + "\n").FontSize(14);
                            
                            x.Item().Text("Pedalo:").FontSize(16);
                            x.Item().Text("Name: " + booking.Pedalo.Name).FontSize(14);
                            x.Item().Text("Color: " + booking.Pedalo.Color).FontColor(ColorHex).FontSize(14);
                            x.Item().Text("Capacity: " + booking.Pedalo.Capacity).FontSize(14);
                            x.Item().Text("Hourly rate: " + booking.Pedalo.HourlyRate + "\n").FontSize(14);
                            
                            x.Item().Text("Dates:").FontSize(16);
                            x.Item().Text("Start date: " + booking.StartDate).FontSize(14);
                            x.Item().Text("End date: " + booking.EndDate + "\n").FontSize(14);

                            x.Item().Text("Passengers:").FontSize(16);
                            foreach (var item in booking.BookingPassengers)
                            {
                                x.Item().Text("-" + item.Passenger.Firstname + " " + item.Passenger.Lastname).FontSize(14);
                            }
                            x.Item().Text("\n").FontSize(14);
                            x.Item().BorderTop(1);
                            x.Item().Image("images/OIP.jpg");
                        });


                });
            })
            .GeneratePdf();

            return File(pdfBytes, "application/pdf");
        }
    }
}
