﻿namespace PedaloWebApp.Pages
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
        private readonly ILogger<IndexModel> _logger;
        private readonly IDbContextFactory contextFactory;


        public IndexModel(ILogger<IndexModel> logger, IDbContextFactory contextFactory)
        {
            this._logger = logger;
            this.contextFactory = contextFactory;
        }


        public IReadOnlyList<Pedalo> Pedaloes { get; set; }

        public IActionResult OnGet()
        {
            using var context = this.contextFactory.CreateReadOnlyContext();
            this.Pedaloes = context.Pedaloes.ToList();
            return this.Page();
        }
    }
}
