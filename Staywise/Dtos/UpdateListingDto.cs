using System.ComponentModel.DataAnnotations;

namespace Staywise.Dtos;

public class UpdateListingDto
{
    public Guid Id{ get; set; }
    
    [MaxLength(150)]
    public string? Title { get; set; }
    [MaxLength(500)]
    public string Description { get; set; }

    [Required]
    public decimal PricePerNight { get; set; }
    [Required]
    public AddressDto? Address { get; set; }
    public IFormFile? Photo { get; set; }
    public List<string> Amenities { get; set; } = new();
    }