using System.ComponentModel.DataAnnotations;

namespace Staywise.Dtos;

public class ListingResponseDto
{
    public Guid Id { get; set; }
    public Guid HostId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal PricePerNight { get; set; }
    public AddressDto Address { get; set; } = new();
}
