using System;
using System.Collections.Generic;

namespace test2.Models;

public partial class Reservation
{
    public int ReservationId { get; set; }

    public int CId { get; set; }

    public int CollectionId { get; set; }

    public int? BookId { get; set; }

    public DateTime ReservationDate { get; set; }

    public DateTime? DueDateR { get; set; }

    public int ReservationStatusId { get; set; }

    public virtual Book? Book { get; set; }

    public virtual ICollection<Borrow> Borrows { get; set; } = new List<Borrow>();

    public virtual Client CIdNavigation { get; set; } = null!;

    public virtual Collection Collection { get; set; } = null!;

    public virtual ReservationStatus ReservationStatus { get; set; } = null!;
}
