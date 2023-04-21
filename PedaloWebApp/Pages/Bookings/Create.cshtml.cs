namespace PedaloWebApp.Pages.Bookings
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.FileSystemGlobbing.Internal;
    using PedaloWebApp.Core.Domain.Entities;
    using PedaloWebApp.Core.Interfaces.Data;
    using Newtonsoft.Json;
    using QuestPDF.Drawing;
    using QuestPDF.Fluent;
    using Microsoft.EntityFrameworkCore;
    using MimeKit;

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
        public string Error { get; set; }

        //public string url = "https://api.open-meteo.com/v1/forecast?latitude=47.37&longitude=8.55&daily=weathercode,temperature_2m_max,temperature_2m_min&forecast_days=3&timezone=auto";


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
                return this.Page();
                
            }

            using var context = this.contextFactory.CreateContext();
            if (this.Booking.EndDate > this.Booking.StartDate || this.Booking.EndDate == null)
            {
                try
                {
                    /*DateTime dateTime = this.Booking.StartDate;
                    DateTime newDateTime = DateTime.ParseExact(dateTime.ToString("yyyy/MM/dd"), "yyyy/MM/dd", null);*/
                
                        var booking = new Booking
                        {
                            CustomerId = this.Booking.CustomerId,
                            PedaloId = this.Booking.PedaloId,
                            StartDate = this.Booking.StartDate,
                            EndDate = this.Booking.EndDate
                        };
                        context.Bookings.Add(booking);
                        context.SaveChanges();
                        var pedalo = context.Pedaloes.Where(x => x.PedaloId == booking.PedaloId).Single();

                        if (pedalo.Capacity != 1)
                        {
                            return this.RedirectToPage("AddPassenger", new { bookingid = booking.BookingId });
                        }
                        else
                        {
                            return this.RedirectToPage("Index");

                        }

                }
                catch (Exception)
                {
                    return this.RedirectToPage("/Error");
                }
            }
            else
            {
                this.Pedalos = context.Pedaloes.OrderBy(x => x.Name).ThenBy(x => x.Color).ToList();
                this.Customer = context.Customers.OrderBy(x => x.FirstName).ThenBy(x => x.LastName).ToList();
                this.Passenger = context.Passengers.OrderBy(x => x.Firstname).ThenBy(x => x.Lastname).ToList();
                Error = "The enddate cannot take place before the startdate";
            }

            return this.Page();

        }

        /*public void GenerateBookingPdf(Booking booking)
        {
            var document = new Document();

            document.AddSection(section =>
            {
                section
                .PageMargins(20)
                .Paragraphs(paragraphs =>
                {
                    paragraphs
                     .Text("Booking Details", TextStyle.Default.Size(18))
                     .LineBreak()
                     .LineBreak()
                     .Text($"Booking ID: {booking.BookingId}")
                     .LineBreak()
                     .Text($"Customer Name: {booking.CustomerName}")
                     .LineBreak()
                     .Text($"Room Type: {booking.RoomType}")
                     .LineBreak()
                     .Text($"Check-in: {booking.CheckIn.ToShortDateString()}")
                     .LineBreak()
                     .Text($"C   heck-out: {booking.CheckOut.ToShortDateString()}")
                     .LineBreak()
                     .Text($"Total Price: {booking.TotalPrice:C}")
                     .LineBreak()
                     .LineBreak()
                     .Text("Thank you for your booking!")
                     .LineBreak();
                });
            });

            var bytes = document.GeneratePdf();
            File.WriteAllBytes("BookingDetails.pdf", bytes); // Save the PDF to a file
        }*/

        

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

