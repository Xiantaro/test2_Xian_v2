using System;
using System.Collections.Generic;

namespace test2.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int CId { get; set; }

    public string Message { get; set; } = null!;

    public DateTime NotificationDate { get; set; }

    public virtual Client CIdNavigation { get; set; } = null!;
}
