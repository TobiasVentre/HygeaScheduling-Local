using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SchedulingMS.Domain.Entities;

namespace SchedulingMS.Infrastructure.Persistence;

public class SchedulingDbContext(DbContextOptions<SchedulingDbContext> options) : DbContext(options)
{
    private static readonly ValueConverter<DateTime, DateTime> UtcDateTimeConverter = new(
        value => value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime(),
        value => DateTime.SpecifyKind(value, DateTimeKind.Utc));

    public DbSet<AvailabilitySlot> AvailabilitySlots => Set<AvailabilitySlot>();
    public DbSet<TechnicianAbsence> TechnicianAbsences => Set<TechnicianAbsence>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<ReservationAssignment> ReservationAssignments => Set<ReservationAssignment>();
    public DbSet<ReservationStatusHistory> ReservationStatusHistory => Set<ReservationStatusHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AvailabilitySlot>(entity =>
        {
            entity.ToTable("AvailabilitySlots");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TechnicianId).IsRequired();
            entity.Property(x => x.ProviderEntityId).IsRequired();
            entity.Property(x => x.StartAtUtc).HasConversion(UtcDateTimeConverter).IsRequired();
            entity.Property(x => x.EndAtUtc).HasConversion(UtcDateTimeConverter).IsRequired();
            entity.HasIndex(x => new { x.TechnicianId, x.StartAtUtc, x.EndAtUtc });
        });

        modelBuilder.Entity<TechnicianAbsence>(entity =>
        {
            entity.ToTable("TechnicianAbsences");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TechnicianId).IsRequired();
            entity.Property(x => x.ProviderEntityId).IsRequired();
            entity.Property(x => x.StartAtUtc).HasConversion(UtcDateTimeConverter).IsRequired();
            entity.Property(x => x.EndAtUtc).HasConversion(UtcDateTimeConverter).IsRequired();
            entity.Property(x => x.Reason).HasMaxLength(255).IsRequired();
            entity.HasIndex(x => new { x.TechnicianId, x.StartAtUtc, x.EndAtUtc });
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.ToTable("Reservations");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ClientId).IsRequired();
            entity.Property(x => x.ProviderEntityId).IsRequired();
            entity.Property(x => x.ServiceId).IsRequired();
            entity.Property(x => x.StartAtUtc).HasConversion(UtcDateTimeConverter).IsRequired();
            entity.Property(x => x.EndAtUtc).HasConversion(UtcDateTimeConverter).IsRequired();
            entity.Property(x => x.CreatedAtUtc).HasConversion(UtcDateTimeConverter).IsRequired();
            entity.Property(x => x.Status).HasConversion<int>().IsRequired();
            entity.HasIndex(x => x.ClientId);
            entity.HasIndex(x => new { x.ProviderEntityId, x.StartAtUtc });
        });

        modelBuilder.Entity<ReservationAssignment>(entity =>
        {
            entity.ToTable("ReservationAssignments");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ReservationId).IsRequired();
            entity.Property(x => x.TechnicianId).IsRequired();
            entity.Property(x => x.AssignmentType).HasConversion<int>().IsRequired();
            entity.Property(x => x.AssignedAtUtc).HasConversion(UtcDateTimeConverter).IsRequired();
            entity.Property(x => x.IsCurrent).IsRequired();
            entity.Property(x => x.Reason).HasMaxLength(255);
            entity.HasIndex(x => new { x.ReservationId, x.IsCurrent });
            entity.HasIndex(x => new { x.TechnicianId, x.IsCurrent });
        });

        modelBuilder.Entity<ReservationStatusHistory>(entity =>
        {
            entity.ToTable("ReservationStatusHistory");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ReservationId).IsRequired();
            entity.Property(x => x.PreviousStatus).HasConversion<int>();
            entity.Property(x => x.NewStatus).HasConversion<int>().IsRequired();
            entity.Property(x => x.ChangedAtUtc).HasConversion(UtcDateTimeConverter).IsRequired();
            entity.Property(x => x.Note).HasMaxLength(255);
            entity.HasIndex(x => new { x.ReservationId, x.ChangedAtUtc });
        });
    }
}


