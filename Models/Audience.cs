using System;
using System.Collections.Generic;

namespace test2.Models;

public partial class Audience
{
    public int AudienceId { get; set; }

    public string Audience1 { get; set; } = null!;

    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
}
