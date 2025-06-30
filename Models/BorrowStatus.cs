using System;
using System.Collections.Generic;

namespace test2.Models;

public partial class BorrowStatus
{
    public int BorrowStatusId { get; set; }

    public string BorrowStatus1 { get; set; } = null!;

    public virtual ICollection<Borrow> Borrows { get; set; } = new List<Borrow>();
}
