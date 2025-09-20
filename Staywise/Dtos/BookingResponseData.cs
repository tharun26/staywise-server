using System.ComponentModel.DataAnnotations;
using Staywise.Enums;

namespace Staywise.Dtos;

public class BookingResponseDto
{
    public Guid Id { get; set; }
    public Guid ListingId { get; set; }
    public Guid GuestId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; }   
}