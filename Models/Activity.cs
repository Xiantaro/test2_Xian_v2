using System;
using System.Collections.Generic;

namespace test2.Models;

public partial class Activity
{
    public int ActivityId { get; set; }

    public string ActivityTitle { get; set; } = null!;

    public string? ActivityDesc { get; set; }

    public byte[] ActivityImg { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int Capacity { get; set; }

    public int ActivityTypeId { get; set; }

    public int AudienceId { get; set; }

    public virtual ActivityType ActivityType { get; set; } = null!;

    public virtual Audience Audience { get; set; } = null!;
}
