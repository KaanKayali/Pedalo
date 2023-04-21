
namespace PedaloWebApp.Pages.Bookings
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using System.IO;
    using System.Linq;
    using OfficeOpenXml;
    using PedaloWebApp.Core.Interfaces.Data;
    using DocumentFormat.OpenXml.Office2010.Excel;
    using DocumentFormat.OpenXml.InkML;
    using System;
    using PedaloWebApp.Core.Domain.Entities;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;

    public class ExcelExportModel : PageModel
    {
        private readonly IDbContextFactory contextFactory;

        public ExcelExportModel(IDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        [BindProperty]
        public BookingEditModel Booking { get; set; }
        public IReadOnlyList<Booking> Bookings { get; set; }
        public IReadOnlyList<Pedalo> Pedalos { get; set; }
        public IReadOnlyList<Passenger> Passengers { get; set; }

        public IActionResult OnGet()
        {

            using var context = this.contextFactory.CreateReadOnlyContext();
            this.Bookings = context.Bookings
                .Include(x => x.Customer)
                .Include(x => x.Pedalo)
                .ToList();
            this.Pedalos = context.Pedaloes.ToList();
            this.Passengers = context.Passengers.ToList();

            var bookings = context.Bookings.ToList();

            // Create a new Excel package
            using (var excelPackage = new ExcelPackage())
            {
                // Add a new worksheet to the Excel package
                var worksheet = excelPackage.Workbook.Worksheets.Add("Bookings");



                // Write headers to the worksheet
                worksheet.Cells["A1"].Value = "BookingId";
                worksheet.Cells["B1"].Value = "CustomerId";
                worksheet.Cells["C1"].Value = "PedaloId";
                worksheet.Cells["D1"].Value = "StartDate";
                worksheet.Cells["E1"].Value = "EndDate";



                // Write data to the worksheet
                for (var i = 0; i < bookings.Count; i++)
                {
                    var booking = bookings[i];
                    worksheet.Cells[$"A{i + 2}"].Value = booking.BookingId;
                    worksheet.Cells[$"B{i + 2}"].Value = booking.CustomerId;
                    worksheet.Cells[$"C{i + 2}"].Value = booking.PedaloId;
                    worksheet.Cells[$"D{i + 2}"].Value = booking.StartDate;
                    worksheet.Cells[$"E{i + 2}"].Value = booking.EndDate;
                }



                // Auto-fit columns
                //worksheet.Cells.AutoFitColumns();



                // Save the Excel package to a memory stream
                using (var stream = new MemoryStream())
                {
                    excelPackage.SaveAs(stream);
                    stream.Seek(0, SeekOrigin.Begin);



                    // Return the Excel file as a download
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "bookings.xlsx");
                }
            }
        }
    }
}
