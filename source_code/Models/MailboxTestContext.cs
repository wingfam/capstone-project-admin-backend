using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MailBoxTest.Models;

public partial class MailboxTestContext : DbContext
{
    public MailboxTestContext()
    {
    }

    public MailboxTestContext(DbContextOptions<MailboxTestContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccessWarning> AccessWarnings { get; set; }

    public virtual DbSet<BookingCode> BookingCodes { get; set; }

    public virtual DbSet<BookingHistory> BookingHistories { get; set; }

    public virtual DbSet<BookingOrder> BookingOrders { get; set; }

    public virtual DbSet<Locker> Lockers { get; set; }

    public virtual DbSet<PackageInfo> PackageInfos { get; set; }

    public virtual DbSet<Resident> Residents { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=SE141050\\HANT;Database=MailboxTest;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccessWarning>(entity =>
        {
            entity.HasKey(e => e.AccessWarningId).HasName("PK__access_w__3EF081BE203332F6");

            entity.ToTable("access_warning");

            entity.Property(e => e.AccessWarningId)
                .HasMaxLength(30)
                .HasColumnName("access_warning_id");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.Id)
                .HasMaxLength(30)
                .HasColumnName("ID");
            entity.Property(e => e.LockerId)
                .HasMaxLength(30)
                .HasColumnName("locker_id");
            entity.Property(e => e.Message)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("message");

            entity.HasOne(d => d.Locker).WithMany(p => p.AccessWarnings)
                .HasForeignKey(d => d.LockerId)
                .HasConstraintName("FK__access_wa__locke__797309D9");
        });

        modelBuilder.Entity<BookingCode>(entity =>
        {
            entity.HasKey(e => e.BookingCodeId).HasName("PK__booking___76A24CB8F350E602");

            entity.ToTable("booking_code");

            entity.HasIndex(e => e.BcodeName, "ix_booking_code_bcode_name");

            entity.HasIndex(e => e.BcodeValidDate, "ix_booking_code_bcode_valid_date");

            entity.Property(e => e.BookingCodeId)
                .HasMaxLength(30)
                .HasColumnName("booking_code_id");
            entity.Property(e => e.BcodeName)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("bcode_name");
            entity.Property(e => e.BcodeValidDate)
                .HasColumnType("datetime")
                .HasColumnName("bcode_valid_date");
            entity.Property(e => e.BookingId)
                .HasMaxLength(30)
                .HasColumnName("booking_id");
            entity.Property(e => e.Id)
                .HasMaxLength(30)
                .HasColumnName("ID");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingCodes)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__booking_c__booki__03F0984C");
        });

        modelBuilder.Entity<BookingHistory>(entity =>
        {
            entity.HasKey(e => e.BookingHistoryId).HasName("PK__booking___B8BBFEBB4CF48045");

            entity.ToTable("booking_history");

            entity.HasIndex(e => e.BcodeName, "ix_booking_history_bcode_name");

            entity.HasIndex(e => e.LockerName, "ix_booking_history_locker_name");

            entity.Property(e => e.BookingHistoryId)
                .HasMaxLength(30)
                .HasColumnName("booking_history_id");
            entity.Property(e => e.BcodeName)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("bcode_name");
            entity.Property(e => e.BookingId)
                .HasMaxLength(30)
                .HasColumnName("booking_id");
            entity.Property(e => e.Id)
                .HasMaxLength(30)
                .HasColumnName("ID");
            entity.Property(e => e.LockerName)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("locker_name");
            entity.Property(e => e.UnlockCode)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("unlock_code");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingHistories)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__booking_h__booki__06CD04F7");
        });

        modelBuilder.Entity<BookingOrder>(entity =>
        {
            entity.HasKey(e => e.BookingOrderId).HasName("PK__booking___9B784DB21BB8CC50");

            entity.ToTable("booking_order");

            entity.HasIndex(e => e.BookingDate, "ix_booking_order_booking_date");

            entity.HasIndex(e => e.BookingValidDate, "ix_booking_order_booking_valid_date");

            entity.Property(e => e.BookingOrderId)
                .HasMaxLength(30)
                .HasColumnName("booking_order_id");
            entity.Property(e => e.BookingDate)
                .HasColumnType("datetime")
                .HasColumnName("booking_date");
            entity.Property(e => e.BookingStatus).HasColumnName("booking_status");
            entity.Property(e => e.BookingValidDate)
                .HasColumnType("datetime")
                .HasColumnName("booking_valid_date");
            entity.Property(e => e.Id)
                .HasMaxLength(30)
                .HasColumnName("ID");
            entity.Property(e => e.LockerId)
                .HasMaxLength(30)
                .HasColumnName("locker_id");
            entity.Property(e => e.ResidentId)
                .HasMaxLength(30)
                .HasColumnName("resident_id");

            entity.HasOne(d => d.Locker).WithMany(p => p.BookingOrders)
                .HasForeignKey(d => d.LockerId)
                .HasConstraintName("FK__booking_o__locke__7C4F7684");

            entity.HasOne(d => d.Resident).WithMany(p => p.BookingOrders)
                .HasForeignKey(d => d.ResidentId)
                .HasConstraintName("FK__booking_o__resid__7D439ABD");
        });

        modelBuilder.Entity<Locker>(entity =>
        {
            entity.HasKey(e => e.LockerId).HasName("PK__locker__66B3FF4C1D572CF5");

            entity.ToTable("locker");

            entity.HasIndex(e => e.LockerName, "ix_locker_locker_name");

            entity.Property(e => e.LockerId)
                .HasMaxLength(30)
                .HasColumnName("locker_id");
            entity.Property(e => e.Id)
                .HasMaxLength(30)
                .HasColumnName("ID");
            entity.Property(e => e.LockerName)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("locker_name");
            entity.Property(e => e.LockerStatus).HasColumnName("locker_status");
            entity.Property(e => e.UcodeValidDate)
                .HasColumnType("datetime")
                .HasColumnName("ucode_valid_date");
            entity.Property(e => e.UnlockCode)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("unlock_code");
        });

        modelBuilder.Entity<PackageInfo>(entity =>
        {
            entity.HasKey(e => e.PackageInfoId).HasName("PK__package___5A23EC56C6D7CE34");

            entity.ToTable("package_info");

            entity.HasIndex(e => e.DeliveryDate, "ix_package_info_delivery_date");

            entity.HasIndex(e => e.PickupDate, "ix_package_info_pickup_date");

            entity.Property(e => e.PackageInfoId)
                .HasMaxLength(30)
                .HasColumnName("package_info_id");
            entity.Property(e => e.DeliveryDate)
                .HasColumnType("datetime")
                .HasColumnName("delivery_date");
            entity.Property(e => e.Id)
                .HasMaxLength(30)
                .HasColumnName("ID");
            entity.Property(e => e.LockerId)
                .HasMaxLength(30)
                .HasColumnName("locker_id");
            entity.Property(e => e.PickupDate)
                .HasColumnType("datetime")
                .HasColumnName("pickup_date");
            entity.Property(e => e.ResidentId)
                .HasMaxLength(30)
                .HasColumnName("resident_id");

            entity.HasOne(d => d.Locker).WithMany(p => p.PackageInfos)
                .HasForeignKey(d => d.LockerId)
                .HasConstraintName("FK__package_i__locke__00200768");

            entity.HasOne(d => d.Resident).WithMany(p => p.PackageInfos)
                .HasForeignKey(d => d.ResidentId)
                .HasConstraintName("FK__package_i__resid__01142BA1");
        });

        modelBuilder.Entity<Resident>(entity =>
        {
            entity.HasKey(e => e.ResidentId).HasName("PK__resident__A5BC2ECEBCD78EFD");

            entity.ToTable("resident");

            entity.HasIndex(e => e.Email, "ix_resident_email").IsUnique();

            entity.HasIndex(e => e.Phone, "ix_resident_phone").IsUnique();

            entity.Property(e => e.ResidentId)
                .HasMaxLength(30)
                .HasColumnName("resident_id");
            entity.Property(e => e.Email)
                .HasMaxLength(120)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Fullname)
                .HasMaxLength(100)
                .HasColumnName("fullname");
            entity.Property(e => e.Id)
                .HasMaxLength(30)
                .HasColumnName("ID");
            entity.Property(e => e.IsAvaiable).HasColumnName("isAvaiable");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("phone");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
