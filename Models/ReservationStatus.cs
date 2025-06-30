using System;
using System.Collections.Generic;

namespace test2.Models;

public partial class ReservationStatus
{
    public int ReservationStatusId { get; set; }

    public string ReservationStatus1 { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
