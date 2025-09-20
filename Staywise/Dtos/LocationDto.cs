using System.ComponentModel.DataAnnotations;

namespace Staywise.Dtos;

public class LocationDto
{
    public string Type { get; set; } = "Point";

    [MinLength(2)]
    [MaxLength(2)]
    public double[] Coordinates { get; set; } = Array.Empty<double>();
}
