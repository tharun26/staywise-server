using System.ComponentModel.DataAnnotations;
using Staywise.Models;

namespace Staywise.Models;

public class Location
{
    public string Type { get; set; } = "Point";
    public double[] Coordinates { get; set; } = Array.Empty<double>();
}