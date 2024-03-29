﻿using System;

namespace PedaloWebApp.Core.Domain.Entities
{
    public class BookingPassenger
    {
        public Guid BookingPassengerId { get; set; } = Guid.NewGuid();
        public Guid BookingId { get; set; }
        public Guid PassengerId { get; set; }
        public Booking Booking { get; set; }
        public Passenger Passenger { get; set; }


    }
}
