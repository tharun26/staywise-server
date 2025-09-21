using Microsoft.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Staywise.Models;

namespace Staywise.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    // Db set appears at this place
    public DbSet<User> Users { get; set; }
    public DbSet<Listing> Listings { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Review> Reviews{ get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Listing>().OwnsOne(l => l.Address, address =>
        {
            address.OwnsOne(a => a.Location);
        });

        modelBuilder.Entity<Listing>().Property(l => l.Amenities).HasColumnType("jsonb");
        modelBuilder.Entity<Listing>().Property(l => l.Photos).HasColumnType("jsonb");

        modelBuilder.Entity<Booking>()
        .Property(b => b.Status)
        .HasConversion<string>();

    }
    
}