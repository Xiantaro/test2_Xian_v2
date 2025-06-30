using System;
using System.Collections.Generic;

namespace test2.Models;

public partial class Favorite
{
    public int FavoritesId { get; set; }

    public int CId { get; set; }

    public int CollectionId { get; set; }

    public virtual Client CIdNavigation { get; set; } = null!;

    public virtual Collection Collection { get; set; } = null!;
}
