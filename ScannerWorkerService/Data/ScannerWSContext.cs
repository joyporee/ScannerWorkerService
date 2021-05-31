using Microsoft.EntityFrameworkCore;
using ScannerWorkerService.Models;

namespace ScannerWorkerService.Data
{
    public class ScannerWSContext : DbContext
    {
        public ScannerWSContext(DbContextOptions<ScannerWSContext> options)
            : base(options)
        {
        }

        public DbSet<Device> Devices { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<ScanType> ScanTypes { get; set; }
        public DbSet<ScanRecords> ScanRecords { get; set; }
        public DbSet<ScanEventErrors> ScanEventErrors { get; set; }
        public DbSet<event_tracker> event_tracker { get; set; }

        public DbSet<ConfigurationValue> ConfigurationValues { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<event_tracker>(entity =>
            {
                entity.ToTable("event_tracker");
                entity.HasKey(e => new { e.AppId }).HasName("PK_event_tracker");
            });

            modelBuilder.Entity<ConfigurationValue>(entity =>
            {
                entity.ToTable("sy_options");
                entity.Property(e => e.Id).HasColumnName("sy_option");
                entity.Property(e => e.Value).HasColumnName("sy_value");
            }); 
            
            modelBuilder.Entity<ScanRecords>(entity =>
            {
                entity.ToTable("ScanRecords");
                entity.HasKey(e => new { e.EventId }).HasName("PK_ScanRecords");
            });

            modelBuilder.Entity<ScanType>(entity =>
            {
                entity.ToTable("ScanType");
                entity.HasKey(e => new { e.Id }).HasName("PK_ScanType");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => new { e.Id }).HasName("PK_Users");
            });

            modelBuilder.Entity<Device>(entity =>
            {
                entity.ToTable("Device");
                entity.HasKey(e => new { e.Id }).HasName("PK_Device");
            });
        }
    }
}
