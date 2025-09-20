using System.ComponentModel.DataAnnotations;

namespace Staywise.Dtos;


public class AddressDto
{
    [Required]
    public string Line1 { get; set; } = string.Empty;
    [Required]
    public string Line2 { get; set; } = string.Empty;
    [Required]
    public string City { get; set; } = string.Empty;
    [Required]
    public string State { get; set; } = string.Empty;
    [Required]
    public string Country { get; set; } = string.Empty;
    [Required]
    public int PostalCode { get; set; }
    [Required]
    public LocationDto Location { get; set; } = new LocationDto();
}
