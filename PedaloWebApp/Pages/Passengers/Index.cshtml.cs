namespace PedaloWebApp.Pages.Passengers
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

        public IReadOnlyList<Passenger> Passenger { get; set; }

        public int Loadingcolumns { get; set; }

        public IActionResult OnGet()
        {
            using var context = this.contextFactory.CreateReadOnlyContext();
            this.Passenger = context.Passengers.ToList();
            if (Passenger.Count > 10)
            {
                Loadingcolumns = 10;
            }
            else
            {
                Loadingcolumns = Passenger.Count;
            }
            return this.Page();
        }

        public void OnPostLoadmore()
        {
            using var context = this.contextFactory.CreateReadOnlyContext();
            this.Passenger = context.Passengers.ToList();

            Loadingcolumns = this.Passenger.Count;
        }
    }
}


