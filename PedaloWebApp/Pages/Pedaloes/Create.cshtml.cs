namespace PedaloWebApp.Pages.Pedaloes
{
    using System;
    using System.Drawing;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using PedaloWebApp.Core.Domain.Entities;
    using PedaloWebApp.Core.Interfaces.Data;

    public class CreateModel : PageModel
    {
        private readonly IDbContextFactory contextFactory;

        public CreateModel(IDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        [BindProperty]
        public PedaloCreateModel Pedalo { get; set; }

        public string Error { get; set; }


        public IActionResult OnGet(Guid? id)
        {
            
            return this.Page();
        }

        public IActionResult OnPost()
        {
            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            using var context = this.contextFactory.CreateContext();

            if (this.Pedalo.Name.Length <= 40)
            {
                try
                {
                    var pedalo = new Pedalo
                    {
                        Name = this.Pedalo.Name,
                        Color = this.Pedalo.Color,
                        Capacity = this.Pedalo.Capacity,
                        HourlyRate = this.Pedalo.HourlyRate
                    };
                    context.Pedaloes.Add(pedalo);
                    context.SaveChanges();
                }
                catch (Exception)
                {
                    return this.RedirectToPage("/Error");
                }

                return this.RedirectToPage("./Index");
            }
            else
            {
                Error = "Name too long";
            }

            return this.Page();


        }
    }

    public class PedaloCreateModel
    {
        public Guid PedaloId { get; set; }
        public string Name { get; set; }
        public PedaloColor Color { get; set; }
        public int Capacity { get; set; }
        public decimal HourlyRate { get; set; }
    }

}
