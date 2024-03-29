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
    using static System.Formats.Asn1.AsnWriter;

    public class DownloadPDFModel : PageModel
    {
        private readonly IDbContextFactory contextFactory;

        public DownloadPDFModel(IDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public string today_string = DateTime.Now.ToString("yyyy-MM-dd");

        [BindProperty]
        public BookingEditModel Booking { get; set; }


        public IActionResult OnGet(Guid? id)
        {
            if (id == null)
            {
                return this.BadRequest();
            }

            using var context = this.contextFactory.CreateReadOnlyContext();

            var booking = context.Bookings
                .Where(m => m.BookingId == id)
                .Include(x => x.BookingPassengers).ThenInclude(x => x.Passenger)
                .Include(x => x.Customer)
                .Include(x => x.Pedalo)
                .FirstOrDefault();

            int ColorValue = Color.FromName($"{booking.Pedalo.Color}").ToArgb();
            string ColorHex = string.Format("{0:x6}", ColorValue);

            int ColorBorder = Color.FromName($"{Color.Gray}").ToArgb();
            string ColorHexBorder = string.Format("{0:x6}", ColorBorder);

            int i = 0;


            // code in your main method
            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(24));



                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(x =>
                    {
                        x.Item().Text("Booking " + today_string).FontSize(18);
                        x.Item().Text("id: " + booking.BookingId).Bold().FontSize(18);
                        x.Item().BorderTop(1);//.BorderColor(ColorHexBorder);
                        x.Item().Text("Customer:").ExtraBlack().FontSize(16);
                        x.Item().Text("Name: " + booking.Customer.FirstName + " " + booking.Customer.LastName + "\n").FontSize(14);

                        x.Item().Text("Pedalo:").ExtraBlack().FontSize(16);
                        x.Item().Text("Name: " + booking.Pedalo.Name).FontSize(14);
                        x.Item().Text("Color: " + booking.Pedalo.Color).FontColor(ColorHex).FontSize(14);
                        x.Item().Text("Capacity: " + booking.Pedalo.Capacity + " People have room on it").FontSize(14);
                        x.Item().Text("Hourly rate: " + booking.Pedalo.HourlyRate + " Fr. \n").FontSize(14);

                        x.Item().Text("Dates:").ExtraBlack().FontSize(16);
                        x.Item().Text("Start date: " + booking.StartDate.ToString("yyyy-MM-dd HH:mm:ss")).FontSize(14);
                        if (booking.EndDate != null)
                        {
                            x.Item().Text("End date: " + booking.EndDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "\n").FontSize(14);
                        }
                        else
                        {
                            x.Item().Text("End date: No defined end date\n").FontSize(14);
                        }

                        x.Item().Text("Passengers:").ExtraBlack().FontSize(16);
                        if (booking.BookingPassengers.Count != 0)
                        {
                            foreach (var item in booking.BookingPassengers.OrderBy(x => x.Passenger.Firstname).ThenBy(p => p.Passenger.Lastname)) 
                            {
                                //container(p =>
                                //{
                                //    while (i < 5)
                                //    {
                                          x.Item().Text("-" + item.Passenger.Firstname + " " + item.Passenger.Lastname).FontSize(14);
                                          //text.Span("-" + item.Passenger.Firstname + " " + item.Passenger.Lastname);
                                //        i++;
                                //    }
                                //});
                                


                            }
                        }
                        else
                        {
                            x.Item().Text("No passengers").FontSize(14);
                        }
                        x.Item().Text("\n").FontSize(14);

                        //x.Item().Image("./Pages/Bookings/images/OIP.jpg");
                    });

                page.Footer()
                    .AlignRight()
                    //.Row(row =>
                    //{
                    //    row.ConstantItem(100)
                    //        .Background("#DDD")
                    //        .Padding(10)
                    //        .ExtendVertical()
                    //        .Text("This column is 100 points wide");
                    .Column(x =>
                    {
                        x.Item().Text("� 2020 - Pedalo Verleih").FontSize(18);
                        x.Item().Text("RentAPedalo").FontSize(18);
                        x.Item().BorderBottom(2);//.BorderColor(ColorHexBorder);
                        x.Item().Text(x =>
                        {
                            x.CurrentPageNumber();
                            x.Span("/");
                            x.TotalPages();
                        });
                    });


                 //container
                 //    .Width(1, Unit.Inch)
                 //    .Image("./Pages/Bookings/images/OIP.jpg", ImageScaling.Resize);
                 //        
                 //
                 });

               
            })
            .GeneratePdf();

            return File(pdfBytes, "application/pdf");
        }
    }
}
