using System;
using System.Collections.Generic;

namespace test2.Models;

public partial class Language
{
    public string LanguageCode { get; set; } = null!;

    public string LanguageZh { get; set; } = null!;

    public string LanguageEn { get; set; } = null!;

    public virtual ICollection<Collection> Collections { get; set; } = new List<Collection>();
}
