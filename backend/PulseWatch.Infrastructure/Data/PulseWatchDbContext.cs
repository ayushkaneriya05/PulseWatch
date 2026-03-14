using Microsoft.EntityFrameworkCore;
using PulseWatch.Domain.Entities;

namespace PulseWatch.Infrastructure.Data;

public class PulseWatchDbContext : DbContext
{
    public PulseWatchDbContext(DbContextOptions<PulseWatchDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<ApiEndpoint> ApiEndpoints => Set<ApiEndpoint>();
    public DbSet<MonitoringLog> MonitoringLogs => Set<MonitoringLog>();
    public DbSet<AlertRecord> AlertRecords => Set<AlertRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── User ──
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Name).HasMaxLength(100).IsRequired();
            entity.Property(u => u.Email).HasMaxLength(200).IsRequired();
            entity.Property(u => u.PasswordHash).IsRequired();
        });

        // ── ApiEndpoint ──
        modelBuilder.Entity<ApiEndpoint>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Name).HasMaxLength(200).IsRequired();
            entity.Property(a => a.Url).HasMaxLength(2000).IsRequired();
            entity.Property(a => a.HttpMethod).HasMaxLength(10).IsRequired();
            entity.Property(a => a.Description).HasMaxLength(500);

            entity.HasOne(a => a.User)
                  .WithMany(u => u.ApiEndpoints)
                  .HasForeignKey(a => a.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ── MonitoringLog ──
        modelBuilder.Entity<MonitoringLog>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.HasIndex(m => m.CheckedAt);
            entity.HasIndex(m => m.ApiEndpointId);

            entity.HasOne(m => m.ApiEndpoint)
                  .WithMany(a => a.MonitoringLogs)
                  .HasForeignKey(m => m.ApiEndpointId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ── AlertRecord ──
        modelBuilder.Entity<AlertRecord>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.AlertType).HasMaxLength(50).IsRequired();
            entity.Property(a => a.Message).HasMaxLength(1000).IsRequired();
            entity.HasIndex(a => a.ApiEndpointId);

            entity.HasOne(a => a.ApiEndpoint)
                  .WithMany(a => a.AlertRecords)
                  .HasForeignKey(a => a.ApiEndpointId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
