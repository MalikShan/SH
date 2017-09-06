using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SH.Models
{
    public partial class smartdbContext : DbContext
    {
        public smartdbContext(DbContextOptions<smartdbContext> option) : base(option)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appliances>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(50);

                entity.Property(e => e.RoomId).HasColumnName("Room_id");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasMaxLength(50);

                entity.Property(e => e.Voltages)
                    .HasColumnName("voltages")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(50);

                entity.Property(e => e.NoOfAppliances).HasColumnName("no of appliances");

                entity.Property(e => e.UserId).HasColumnName("user_id");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Action)
                    .HasColumnName("action")
                    .HasMaxLength(50);

                entity.Property(e => e.AppId).HasColumnName("app_id");

                entity.Property(e => e.Day)
                    .HasColumnName("day")
                    .HasMaxLength(50);

                entity.Property(e => e.Inspect)
                    .HasColumnName("inspect")
                    .HasMaxLength(50);

                entity.Property(e => e.Permission)
                    .HasColumnName("permission")
                    .HasMaxLength(50);

                entity.Property(e => e.Request)
                    .HasColumnName("request")
                    .HasColumnType("datetime");
            });
        }

        public virtual DbSet<Appliances> Appliances { get; set; }
        public virtual DbSet<Room> Room { get; set; }
        public virtual DbSet<Transaction> Transaction { get; set; }
    }
}