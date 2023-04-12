namespace PedaloWebApp.Core.Domain.Entities
{
    using System;
    using System.Collections.Generic;

    public class Passenger
    {
        public Guid PassengerId { get; set; } = Guid.NewGuid();
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public ICollection<BookingPassenger> BookingPassengers { get; set; }
    }
}
