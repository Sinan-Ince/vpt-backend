using Microsoft.EntityFrameworkCore;
using VPT.Api.Models;

namespace VPT.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base (options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<VehicleSearch> VehicleSearches { get; set; }

    public DbSet<VehicleTracking> VehicleTrackings { get; set; }

    public DbSet<Listing> Listings { get; set; }

    public DbSet<Notification> Notifications { get; set; }

    public DbSet<VehicleMatch> VehicleMatches { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // User -> VehicleSearch
        modelBuilder.Entity<VehicleSearch>()
            .HasOne(vs => vs.User)
            .WithMany(u => u.VehicleSearches)
            .HasForeignKey(vs => vs.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // VehicleSearch -> VehicleTracking
        modelBuilder.Entity<VehicleTracking>()
            .HasOne(vt => vt.VehicleSearch)
            .WithOne(vs => vs.VehicleTracking)
            .HasForeignKey<VehicleTracking>(vt => vt.VehicleSearchId)
            .OnDelete(DeleteBehavior.Cascade);

        // VehicleTracking -> Notification
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.VehicleTracking)
            .WithMany(vt => vt.Notifications)
            .HasForeignKey(n => n.VehicleTrackingId)
            .OnDelete(DeleteBehavior.Cascade);

        // Listing -> Notification
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Listing)
            .WithMany(l => l.Notifications)
            .HasForeignKey(n => n.ListingId)
            .OnDelete(DeleteBehavior.Cascade);

        // VehicleSearch -> VehicleMatch
        modelBuilder.Entity<VehicleMatch>()
            .HasOne(vm => vm.VehicleSearch)
            .WithMany()
            .HasForeignKey(vm => vm.VehicleSearchId)
            .OnDelete(DeleteBehavior.Cascade);

        // Listing -> VehicleMatch
        modelBuilder.Entity<VehicleMatch>()
            .HasOne(vm => vm.Listing)
            .WithMany(l => l.VehicleMatches)
            .HasForeignKey(vm => vm.ListingId)
            .OnDelete(DeleteBehavior.Cascade);
    }

}
