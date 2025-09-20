using System.ComponentModel.DataAnnotations;

namespace Staywise.Dtos;

public class UpdateListingDto
    {
        [MaxLength(150)]
        public string? Title { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
        public decimal? PricePerNight { get; set; }
        public AddressDto? Address { get; set; }
    }