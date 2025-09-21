using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace Staywise.Models;

// Add unique index to Email
[Index(nameof(Email), IsUnique = true)]
public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string PasswordHash { get; set; } = string.Empty;

}