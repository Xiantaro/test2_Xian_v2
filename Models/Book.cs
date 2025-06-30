using System;
using System.Collections.Generic;

namespace test2.Models;

public partial class Book
{
    public int BookId { get; set; }

    public int CollectionId { get; set; }

    public string BookCode { get; set; } = null!;

    public int BookStatusId { get; set; }

    public DateTime AccessionDate { get; set; }

    public virtual BookStatus BookStatus { get; set; } = null!;

    public virtual ICollection<Borrow> Borrows { get; set; } = new List<Borrow>();

    public virtual Collection Collection { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
