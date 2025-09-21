using System.ComponentModel.DataAnnotations;

namespace Staywise.Models;

public class CreateReviewDto
{
    [Required]
    public Guid ListingId { get; set; }
    [Required]
    public Guid BookingId { get; set; }
    [Required]
    public int Rating { get; set; }
    [Required]
    public string Comment { get; set; } = string.Empty;
}