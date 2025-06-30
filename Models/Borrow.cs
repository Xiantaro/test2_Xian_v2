using System;
using System.Collections.Generic;

namespace test2.Models;

public partial class Borrow
{
    public int BorrowId { get; set; }

    public int CId { get; set; }

    public int? ReservationId { get; set; }

    public int BookId { get; set; }

    public DateTime BorrowDate { get; set; }

    public DateTime DueDateB { get; set; }

    public DateTime? ReturnDate { get; set; }

    public int BorrowStatusId { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual BorrowStatus BorrowStatus { get; set; } = null!;

    public virtual Client CIdNavigation { get; set; } = null!;

    public virtual ICollection<History> Histories { get; set; } = new List<History>();

    public virtual Reservation? Reservation { get; set; }
}
