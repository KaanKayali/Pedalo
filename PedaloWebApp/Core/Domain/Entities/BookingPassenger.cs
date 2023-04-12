using System;

namespace PedaloWebApp.Core.Domain.Entities
{
    public class BookingPassenger
    {
        public Guid BookingPassengerId { get; set; } = Guid.NewGuid();
        public Booking Booking { get; set; }
        public Passenger Passenger { get; set; }


    }
}
