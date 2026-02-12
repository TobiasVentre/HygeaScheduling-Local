using Microsoft.EntityFrameworkCore;
using SchedulingMS.Domain.Entities;

namespace SchedulingMS.Infrastructure.Persistence;

public class SchedulingDbContext(DbContextOptions<SchedulingDbContext> options) : DbContext(options)
{
    public DbSet<AvailabilitySlot> AvailabilitySlots => Set<AvailabilitySlot>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AvailabilitySlot>(entity =>
        {
            entity.ToTable("AvailabilitySlots");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.TechnicianId).IsRequired();
            entity.Property(x => x.StartAtUtc).IsRequired();
            entity.Property(x => x.EndAtUtc).IsRequired();

            entity.HasIndex(x => new { x.TechnicianId, x.StartAtUtc, x.EndAtUtc });
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.ToTable("Reservations");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.ClientId).IsRequired();
            entity.Property(x => x.TechnicianId).IsRequired();
            entity.Property(x => x.ServiceId).IsRequired();
            entity.Property(x => x.StartAtUtc).IsRequired();
            entity.Property(x => x.EndAtUtc).IsRequired();
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(32).IsRequired();

            entity.HasIndex(x => new { x.TechnicianId, x.StartAtUtc, x.EndAtUtc });
            entity.HasIndex(x => x.ClientId);
        });
    }
}
