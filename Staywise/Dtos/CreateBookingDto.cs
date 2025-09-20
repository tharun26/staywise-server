using System.ComponentModel.DataAnnotations;
using Staywise.Enums;

namespace Staywise.Dtos;

public class CreateBookingDto
{
    [Required]
    public Guid ListingId { get; set; }
    [Required]
    public Guid GuestId { get; set; }
    [Required]
    public DateTime CheckIn { get; set; }
    [Required]
    public DateTime CheckOut { get; set; }
    [Required]
    public decimal TotalPrice { get; set; }    
}