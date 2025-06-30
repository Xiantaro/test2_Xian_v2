using System;
using System.Collections.Generic;

namespace test2.Models;

public partial class AnnouncementType
{
    public int AnnouncementTypeId { get; set; }

    public string AnnouncementType1 { get; set; } = null!;

    public virtual ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();
}
