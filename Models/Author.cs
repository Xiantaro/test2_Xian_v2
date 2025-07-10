using System;
using System.Collections.Generic;

namespace test2.Models;

public partial class Author
{
    public int AuthorId { get; set; }

    public string Author1 { get; set; } = null!;

    public string? AuthorDesc { get; set; }

    public virtual ICollection<Collection> Collections { get; set; } = new List<Collection>();
}
