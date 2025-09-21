using System.ComponentModel.DataAnnotations;

namespace Staywise.Models;

public class Review
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    public Guid ListingId { get; set; }
    [Required]
    public Guid BookingId { get; set; }
    [Required]
    public Guid AuthorId { get; set; }
    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }
    [Required]
    public string Comment { get; set; } = string.Empty;

    public Listing? Listing { get; set; }
    public Booking? Booking { get; set; }
    public User? Author{ get; set; }

}