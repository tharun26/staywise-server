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

    // public DbSet<Booking> Bookings { get; set; }
    // public DbSet<Listing> Listings{ get; set; }
}