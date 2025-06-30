using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace test2.Models;

public partial class Test2Context : DbContext
{
    public Test2Context()
    {
    }

    public Test2Context(DbContextOptions<Test2Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Activity> Activities { get; set; }

    public virtual DbSet<ActivityType> ActivityTypes { get; set; }

    public virtual DbSet<Announcement> Announcements { get; set; }

    public virtual DbSet<AnnouncementType> AnnouncementTypes { get; set; }

    public virtual DbSet<Audience> Audiences { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BookStatus> BookStatuses { get; set; }

    public virtual DbSet<Borrow> Borrows { get; set; }

    public virtual DbSet<BorrowStatus> BorrowStatuses { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Collection> Collections { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<History> Histories { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<ReservationStatus> ReservationStatuses { get; set; }

    public virtual DbSet<Type> Types { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=XIANN\\SQLEXPRESS02;Database=test2;Integrated Security=True;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Activity>(entity =>
        {
            entity.ToTable("Activity");

            entity.HasIndex(e => e.ActivityImg, "UQ__Activity__4442C4044387BB23").IsUnique();

            entity.HasIndex(e => e.ActivityTitle, "UQ__Activity__5F9FE5736B1A36C0").IsUnique();

            entity.Property(e => e.ActivityId).HasColumnName("activityId");
            entity.Property(e => e.ActivityDesc).HasColumnName("activityDesc");
            entity.Property(e => e.ActivityImg)
                .HasMaxLength(500)
                .HasColumnName("activityImg");
            entity.Property(e => e.ActivityTitle)
                .HasMaxLength(100)
                .HasColumnName("activityTitle");
            entity.Property(e => e.ActivityTypeId).HasColumnName("activityTypeId");
            entity.Property(e => e.AudienceId).HasColumnName("audienceId");
            entity.Property(e => e.EndDate).HasColumnName("endDate");
            entity.Property(e => e.StartDate).HasColumnName("startDate");

            entity.HasOne(d => d.ActivityType).WithMany(p => p.Activities)
                .HasForeignKey(d => d.ActivityTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ActivityType");

            entity.HasOne(d => d.Audience).WithMany(p => p.Activities)
                .HasForeignKey(d => d.AudienceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Audience");
        });

        modelBuilder.Entity<ActivityType>(entity =>
        {
            entity.ToTable("ActivityType");

            entity.HasIndex(e => e.ActivityType1, "UQ__Activity__1F1EE4DD197FD4FC").IsUnique();

            entity.Property(e => e.ActivityTypeId).HasColumnName("activityTypeId");
            entity.Property(e => e.ActivityType1)
                .HasMaxLength(50)
                .HasColumnName("activityType");
        });

        modelBuilder.Entity<Announcement>(entity =>
        {
            entity.ToTable("Announcement");

            entity.HasIndex(e => e.AnnouncementTitle, "UQ__Announce__EBE0015F6B95E4E9").IsUnique();

            entity.Property(e => e.AnnouncementId).HasColumnName("announcementId");
            entity.Property(e => e.AnnouncementDesc).HasColumnName("announcementDesc");
            entity.Property(e => e.AnnouncementTitle)
                .HasMaxLength(100)
                .HasColumnName("announcementTitle");
            entity.Property(e => e.AnnouncementTypeId).HasColumnName("announcementTypeId");

            entity.HasOne(d => d.AnnouncementType).WithMany(p => p.Announcements)
                .HasForeignKey(d => d.AnnouncementTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AnnouncementType");
        });

        modelBuilder.Entity<AnnouncementType>(entity =>
        {
            entity.ToTable("AnnouncementType");

            entity.HasIndex(e => e.AnnouncementType1, "UQ__Announce__D53C87B213E8BA32").IsUnique();

            entity.Property(e => e.AnnouncementTypeId).HasColumnName("announcementTypeId");
            entity.Property(e => e.AnnouncementType1)
                .HasMaxLength(50)
                .HasColumnName("announcementType");
        });

        modelBuilder.Entity<Audience>(entity =>
        {
            entity.ToTable("Audience");

            entity.HasIndex(e => e.Audience1, "UQ__Audience__2C1B51FCC7A9A3D7").IsUnique();

            entity.Property(e => e.AudienceId).HasColumnName("audienceId");
            entity.Property(e => e.Audience1)
                .HasMaxLength(50)
                .HasColumnName("audience");
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.ToTable("Book");

            entity.HasIndex(e => e.BookCode, "UQ__Book__601375FAA32892F8").IsUnique();

            entity.Property(e => e.BookId).HasColumnName("bookId");
            entity.Property(e => e.AccessionDate).HasColumnName("accessionDate");
            entity.Property(e => e.BookCode)
                .HasMaxLength(23)
                .IsUnicode(false)
                .HasColumnName("bookCode");
            entity.Property(e => e.BookStatusId).HasColumnName("bookStatusId");
            entity.Property(e => e.CollectionId).HasColumnName("collectionId");

            entity.HasOne(d => d.BookStatus).WithMany(p => p.Books)
                .HasForeignKey(d => d.BookStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookStatus");

            entity.HasOne(d => d.Collection).WithMany(p => p.Books)
                .HasForeignKey(d => d.CollectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CollectionB");
        });

        modelBuilder.Entity<BookStatus>(entity =>
        {
            entity.ToTable("BookStatus");

            entity.HasIndex(e => e.BookStatus1, "UQ__BookStat__6890475C4F4AD476").IsUnique();

            entity.Property(e => e.BookStatusId).HasColumnName("bookStatusId");
            entity.Property(e => e.BookStatus1)
                .HasMaxLength(50)
                .HasColumnName("bookStatus");
        });

        modelBuilder.Entity<Borrow>(entity =>
        {
            entity.ToTable("Borrow");

            entity.Property(e => e.BorrowId).HasColumnName("borrowId");
            entity.Property(e => e.BookId).HasColumnName("bookId");
            entity.Property(e => e.BorrowDate).HasColumnName("borrowDate");
            entity.Property(e => e.BorrowStatusId).HasColumnName("borrowStatusId");
            entity.Property(e => e.CId).HasColumnName("cId");
            entity.Property(e => e.DueDateB).HasColumnName("dueDateB");
            entity.Property(e => e.ReservationId).HasColumnName("reservationId");
            entity.Property(e => e.ReturnDate).HasColumnName("returnDate");

            entity.HasOne(d => d.Book).WithMany(p => p.Borrows)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookB");

            entity.HasOne(d => d.BorrowStatus).WithMany(p => p.Borrows)
                .HasForeignKey(d => d.BorrowStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BorrowStatus");

            entity.HasOne(d => d.CIdNavigation).WithMany(p => p.Borrows)
                .HasForeignKey(d => d.CId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClientB");

            entity.HasOne(d => d.Reservation).WithMany(p => p.Borrows)
                .HasForeignKey(d => d.ReservationId)
                .HasConstraintName("FK_Reservation");
        });

        modelBuilder.Entity<BorrowStatus>(entity =>
        {
            entity.ToTable("BorrowStatus");

            entity.HasIndex(e => e.BorrowStatus1, "UQ__BorrowSt__EF08F8F4E23C4D12").IsUnique();

            entity.Property(e => e.BorrowStatusId).HasColumnName("borrowStatusId");
            entity.Property(e => e.BorrowStatus1)
                .HasMaxLength(50)
                .HasColumnName("borrowStatus");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.CId);

            entity.ToTable("Client");

            entity.HasIndex(e => e.CPhone, "UQ__Client__0A376ADC2AB63127").IsUnique();

            entity.HasIndex(e => e.CAccount, "UQ__Client__2F046D2324CA387F").IsUnique();

            entity.HasIndex(e => e.CId, "UQ__Client__D830D47658CAB459").IsUnique();

            entity.Property(e => e.CId)
                .ValueGeneratedNever()
                .HasColumnName("cId");
            entity.Property(e => e.CAccount)
                .HasMaxLength(100)
                .HasColumnName("cAccount");
            entity.Property(e => e.CName)
                .HasMaxLength(50)
                .HasColumnName("cName");
            entity.Property(e => e.CPassword)
                .HasMaxLength(64)
                .HasColumnName("cPassword");
            entity.Property(e => e.CPhone)
                .HasMaxLength(20)
                .HasColumnName("cPhone");
            entity.Property(e => e.Permission).HasColumnName("permission");
        });

        modelBuilder.Entity<Collection>(entity =>
        {
            entity.ToTable("Collection");

            entity.HasIndex(e => e.CollectionImg, "UQ__Collecti__5A4F4868D2DF774E").IsUnique();

            entity.HasIndex(e => e.Isbn, "UQ__Collecti__99F9D0A49F918064").IsUnique();

            entity.Property(e => e.CollectionId).HasColumnName("collectionId");
            entity.Property(e => e.Author)
                .HasMaxLength(50)
                .HasColumnName("author");
            entity.Property(e => e.CollectionDesc).HasColumnName("collectionDesc");
            entity.Property(e => e.CollectionImg)
                .HasMaxLength(500)
                .HasColumnName("collectionImg");
            entity.Property(e => e.Isbn)
                .HasMaxLength(17)
                .IsUnicode(false)
                .HasColumnName("isbn");
            entity.Property(e => e.LanguageCode)
                .HasMaxLength(10)
                .HasColumnName("languageCode");
            entity.Property(e => e.PublishDate).HasColumnName("publishDate");
            entity.Property(e => e.Publisher)
                .HasMaxLength(50)
                .HasColumnName("publisher");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");
            entity.Property(e => e.Translator)
                .HasMaxLength(50)
                .HasColumnName("translator");
            entity.Property(e => e.TypeId).HasColumnName("typeId");

            entity.HasOne(d => d.LanguageCodeNavigation).WithMany(p => p.Collections)
                .HasForeignKey(d => d.LanguageCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Language");

            entity.HasOne(d => d.Type).WithMany(p => p.Collections)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Type");
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => e.FavoritesId);

            entity.HasIndex(e => new { e.CId, e.CollectionId }, "CUC_Favorites").IsUnique();

            entity.Property(e => e.FavoritesId).HasColumnName("favoritesId");
            entity.Property(e => e.CId).HasColumnName("cId");
            entity.Property(e => e.CollectionId).HasColumnName("collectionId");

            entity.HasOne(d => d.CIdNavigation).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.CId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClientF");

            entity.HasOne(d => d.Collection).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.CollectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CollectionF");
        });

        modelBuilder.Entity<History>(entity =>
        {
            entity.ToTable("History");

            entity.Property(e => e.HistoryId).HasColumnName("historyId");
            entity.Property(e => e.BorrowId).HasColumnName("borrowId");
            entity.Property(e => e.Feedback).HasColumnName("feedback");
            entity.Property(e => e.Score).HasColumnName("score");

            entity.HasOne(d => d.Borrow).WithMany(p => p.Histories)
                .HasForeignKey(d => d.BorrowId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Borrow");
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(e => e.LanguageCode);

            entity.ToTable("Language");

            entity.Property(e => e.LanguageCode)
                .HasMaxLength(10)
                .HasColumnName("languageCode");
            entity.Property(e => e.LanguageEn)
                .HasMaxLength(50)
                .HasColumnName("languageEn");
            entity.Property(e => e.LanguageZh)
                .HasMaxLength(50)
                .HasColumnName("languageZh");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("Notification");

            entity.Property(e => e.NotificationId).HasColumnName("notificationId");
            entity.Property(e => e.CId).HasColumnName("cId");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.NotificationDate).HasColumnName("notificationDate");

            entity.HasOne(d => d.CIdNavigation).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.CId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClientN");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.ToTable("Reservation");

            entity.Property(e => e.ReservationId).HasColumnName("reservationId");
            entity.Property(e => e.BookId).HasColumnName("bookId");
            entity.Property(e => e.CId).HasColumnName("cId");
            entity.Property(e => e.CollectionId).HasColumnName("collectionId");
            entity.Property(e => e.DueDateR).HasColumnName("dueDateR");
            entity.Property(e => e.ReservationDate).HasColumnName("reservationDate");
            entity.Property(e => e.ReservationStatusId).HasColumnName("reservationStatusId");

            entity.HasOne(d => d.Book).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("FK_BookR");

            entity.HasOne(d => d.CIdNavigation).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.CId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClientR");

            entity.HasOne(d => d.Collection).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.CollectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CollectionR");

            entity.HasOne(d => d.ReservationStatus).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.ReservationStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReservationStatus");
        });

        modelBuilder.Entity<ReservationStatus>(entity =>
        {
            entity.ToTable("ReservationStatus");

            entity.HasIndex(e => e.ReservationStatus1, "UQ__Reservat__C351879361DCDF39").IsUnique();

            entity.Property(e => e.ReservationStatusId).HasColumnName("reservationStatusId");
            entity.Property(e => e.ReservationStatus1)
                .HasMaxLength(50)
                .HasColumnName("reservationStatus");
        });

        modelBuilder.Entity<Type>(entity =>
        {
            entity.ToTable("Type");

            entity.HasIndex(e => e.Type1, "UQ__Type__E3F85248D2F545C1").IsUnique();

            entity.Property(e => e.TypeId).HasColumnName("typeId");
            entity.Property(e => e.Type1)
                .HasMaxLength(50)
                .HasColumnName("type");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
