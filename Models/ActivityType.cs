using System;
using System.Collections.Generic;

namespace test2.Models;

public partial class ActivityType
{
    public int ActivityTypeId { get; set; }

    public string ActivityType1 { get; set; } = null!;

    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
}
