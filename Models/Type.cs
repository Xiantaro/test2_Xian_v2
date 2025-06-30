using System;
using System.Collections.Generic;

namespace test2.Models;

public partial class Type
{
    public int TypeId { get; set; }

    public string Type1 { get; set; } = null!;

    public virtual ICollection<Collection> Collections { get; set; } = new List<Collection>();
}
