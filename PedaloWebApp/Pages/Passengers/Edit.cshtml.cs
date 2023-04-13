namespace PedaloWebApp.Pages.Passengers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using PedaloWebApp.Core.Domain.Entities;
    using PedaloWebApp.Core.Interfaces.Data;

    public class EditModel : PageModel
    {
        private readonly IDbContextFactory contextFactory;

        public EditModel(IDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        [BindProperty]
        public PassengerEditModel Passenger { get; set; }

        public IActionResult OnGet(Guid? id)
        {
            if (id == null)
            {
                return this.BadRequest();
            }

            using var context = this.contextFactory.CreateReadOnlyContext();
            this.Passenger = context.Passengers
                .Where(m => m.PassengerId == id)
                .Select(x => new PassengerEditModel
                {
                    PassengerId = x.PassengerId,
                    Firstname = x.Firstname,
                    Lastname = x.Lastname
                })
                .FirstOrDefault();

            if (this.Passenger == null)
            {
                return this.NotFound();
            }

            return this.Page();

        }

        public IActionResult OnPost()
        {
            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            using var context = this.contextFactory.CreateContext();
            var passenger = context.Passengers.FirstOrDefault(x => x.PassengerId == this.Passenger.PassengerId);
            if (passenger == null)
            {
                return this.NotFound();
            }

            try
            {
                passenger.PassengerId = this.Passenger.PassengerId;
                passenger.Firstname = this.Passenger.Firstname;
                passenger.Lastname = this.Passenger.Lastname;

                context.SaveChanges();
            }
            catch (Exception)
            {
                return this.RedirectToPage("/Error");
            }

            return this.RedirectToPage("./Index");
        }
    }

    public class PassengerEditModel
    {
        public Guid PassengerId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }
}
