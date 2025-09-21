using System.ComponentModel.DataAnnotations;

namespace Staywise.Models;

public class Address
{
    public Guid Id { get; set; } = Guid.NewGuid();

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
    public Location Location { get; set; } = new Location();
}