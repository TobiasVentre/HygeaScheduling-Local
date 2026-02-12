using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SchedulingMS.Infrastructure.Persistence;

#nullable disable

namespace SchedulingMS.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(SchedulingDbContext))]
    partial class SchedulingDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SchedulingMS.Domain.Entities.AvailabilitySlot", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("EndAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("StartAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("TechnicianId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TechnicianId", "StartAtUtc", "EndAtUtc");

                    b.ToTable("AvailabilitySlots", (string)null);
                });

            modelBuilder.Entity("SchedulingMS.Domain.Entities.Reservation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("ClientId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("EndAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ServiceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("StartAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<int>("TechnicianId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("TechnicianId", "StartAtUtc", "EndAtUtc");

                    b.ToTable("Reservations", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
