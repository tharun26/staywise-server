
using System.ComponentModel.DataAnnotations;

namespace Staywise.Dtos;

public class SignUpDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    [MinLength(5)]
    public string Password { get; set; } = string.Empty;
}