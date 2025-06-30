using System;
using System.Collections.Generic;

namespace test2.Models;

public partial class History
{
    public int HistoryId { get; set; }

    public int BorrowId { get; set; }

    public int? Score { get; set; }

    public string? Feedback { get; set; }

    public virtual Borrow Borrow { get; set; } = null!;
}
