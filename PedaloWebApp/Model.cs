﻿using PedaloWebApp.Core.Domain.Entities;

namespace PedaloWebApp
{
    public class PedaloDashboard
    {
        public string PedaloName { get; set; }
        public int TotalBookings { get; set; }

    }

    public class PedaloCustomerDashboard
    {
        public string CustomerName { get; set; }
        public int TotalCustomerBookings { get; set; }

    }
}