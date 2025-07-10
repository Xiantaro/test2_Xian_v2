using System;
using System.Collections.Generic;

namespace test2.Models;

public partial class Collection
{
    public int CollectionId { get; set; }

    public string Title { get; set; } = null!;

    public string? CollectionDesc { get; set; }

    public byte[] CollectionImg { get; set; } = null!;

    public int TypeId { get; set; }

    public string? Translator { get; set; }

    public string Publisher { get; set; } = null!;

    public int LanguageId { get; set; }

    public string Isbn { get; set; } = null!;

    public DateTime PublishDate { get; set; }

    public int AuthorId { get; set; }

    public virtual Author Author { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual Language Language { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual Type Type { get; set; } = null!;
}
