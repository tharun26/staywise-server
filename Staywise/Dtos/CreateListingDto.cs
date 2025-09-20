using System.ComponentModel.DataAnnotations;

namespace Staywise.Dtos;

public class CreateListingDto
{
    [Required, MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public decimal PricePerNight { get; set; }

    [Required]
    public AddressDto Address { get; set; } = new();
}