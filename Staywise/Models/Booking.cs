
using System.ComponentModel.DataAnnotations;
using Staywise.Enums;

namespace Staywise.Models;

public class Booking
{
    public Guid Id { get; set; } = Guid.NewGuid();

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
    [Required]
    public Status Status { get; set; }

    public User? Guest { get; set; }

}