using System.ComponentModel.DataAnnotations;

namespace Staywise.Models;

public class ReviewResponseDto
{
    public Guid Id { get; set; }
    public Guid ListingId { get; set; }
    public Guid BookingId { get; set; }
    public Guid UserId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
}