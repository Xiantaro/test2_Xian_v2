using System;
using System.Collections.Generic;

namespace test2.Models;

public partial class Announcement
{
    public int AnnouncementId { get; set; }

    public int AnnouncementTypeId { get; set; }

    public string AnnouncementTitle { get; set; } = null!;

    public string? AnnouncementDesc { get; set; }

    public DateTime Date { get; set; }

    public virtual AnnouncementType AnnouncementType { get; set; } = null!;
}
