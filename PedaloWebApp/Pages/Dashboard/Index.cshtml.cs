namespace PedaloWebApp.Pages.Dashboard
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using PedaloWebApp.Core.Domain.Entities;
    using PedaloWebApp.Core.Interfaces.Data;

    public class IndexModel : PageModel
    {
        private readonly IDbContextFactory contextFactory;

        public IndexModel(IDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public IReadOnlyList<PedaloDashboard> Pedaloes { get; set; }
        public IReadOnlyList<PedaloCustomerDashboard> Customers { get; set; }
        public List<Pedalo> Pedalos { get; set; }

        public IActionResult OnGet()
        {
            using var context = this.contextFactory.CreateReadOnlyContext();
            this.Pedaloes = context.Pedaloes.Select(x => new PedaloDashboard
            {
                PedaloName = x.Name,
                TotalBookings = x.Bookings.Count(),
            }).OrderByDescending(x => x.TotalBookings).Take(5).ToList();
            this.Pedalos = context.Pedaloes.ToList();
            this.Customers = context.Customers.Select(x => new PedaloCustomerDashboard
            {
                CustomerName = x.FirstName,
                TotalCustomerBookings = x.Bookings.Count(),
            }).OrderByDescending(x => x.TotalCustomerBookings).Take(5).ToList();
            return this.Page();
        }
    }

}
