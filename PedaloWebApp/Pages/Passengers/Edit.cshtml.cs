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
        public PassengersEditModel Passengers { get; set; }

        public IActionResult OnGet(Guid? id)
        {
            if (id == null)
            {
                return this.BadRequest();
            }

            using var context = this.contextFactory.CreateReadOnlyContext();
            this.Passengers = context.Passengers
                .Where(m => m.PassengerId == id)
                .Select(x => new PassengersEditModel
                {
                    PassengerId = x.PassengerId,
                    Firstname = x.Firstname,
                    Lastname = x.Lastname
                })
                .FirstOrDefault();

            if (this.Passengers == null)
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
            var passenger = context.Passengers.FirstOrDefault(x => x.PassengerId == this.Passengers.PassengerId);
            if (passenger == null)
            {
                return this.NotFound();
            }

            try
            {
                passenger.PassengerId = this.Passengers.PassengerId;
                passenger.Firstname = this.Passengers.Firstname;
                passenger.Lastname = this.Passengers.Lastname;

                context.SaveChanges();
            }
            catch (Exception)
            {
                return this.RedirectToPage("/Error");
            }

            return this.RedirectToPage("./Index");
        }
    }

    public class PassengersEditModel
    {
        public Guid PassengerId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }
}
