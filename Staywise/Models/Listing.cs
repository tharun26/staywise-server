using System.ComponentModel.DataAnnotations;

namespace Staywise.Models;

public class Listing
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid HostId { get; set; }
    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0, 10000)]
    public decimal PricePerNight { get; set; }

    [Required]
    public Guid AddressId { get; set; }
    public List<string> Amenities { get; set; } = new();

    public List<string> Photos { get; set; } = new();

    [Range(1, 10)]
    public int MaxGuests { get; set; }

    public int BedRooms { get; set; }
    public int BathRooms { get; set; }

    //Navigation property
    public User? Host { get; set; }
    public Address? Address { get; set; } = new();

}