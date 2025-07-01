using System;
using System.Collections.Generic;

namespace test2.Models;

public partial class Language
{
    public int LanguageId { get; set; }

    public string Language1 { get; set; } = null!;

    public virtual ICollection<Collection> Collections { get; set; } = new List<Collection>();
}
