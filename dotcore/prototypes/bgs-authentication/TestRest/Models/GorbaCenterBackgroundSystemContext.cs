using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TestDB.Models
{
    public partial class GorbaCenterBackgroundSystemContext : DbContext
    {
        public GorbaCenterBackgroundSystemContext()
        {
        }

        public GorbaCenterBackgroundSystemContext(DbContextOptions<GorbaCenterBackgroundSystemContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=TZULHASNINE-Z42\\SQLEXPRESS;Database=Gorba.Center.BackgroundSystem;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasIndex(e => e.OwnerTenantId)
                    .HasName("IX_OwnerTenant_Id");

                entity.HasIndex(e => e.Username)
                    .HasName("IX_Username")
                    .IsUnique();

                entity.Property(e => e.OwnerTenantId).HasColumnName("OwnerTenant_Id");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Version).HasDefaultValueSql("((1))");
            });
        }
    }
}
