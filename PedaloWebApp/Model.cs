using PedaloWebApp.Core.Domain.Entities;
using System;

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

    public class BookingPassengerAmount
    {
        public Guid BookingId { get; set; }
        public string PassengerName { get; set; }
        public int TotalPassengers { get; set; }

    }
}
