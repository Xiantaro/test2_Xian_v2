using System;
using System.Collections.Generic;

namespace test2.Models;

public partial class BookStatus
{
    public int BookStatusId { get; set; }

    public string BookStatus1 { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
