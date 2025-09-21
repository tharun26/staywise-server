using System.ComponentModel.DataAnnotations;
using Staywise.Models;

namespace Staywise.Dtos;

public class ListingResponseDto
{
    public Guid Id { get; set; }
    public Guid HostId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal PricePerNight { get; set; }
    public AddressDto Address { get; set; } = new();
    public List<string> Amenities { get; set; } = new();
    public List<string> Photos { get; set; } = new();

    public User? Host{ get; set; }

}
