using System.ComponentModel.DataAnnotations;
using Staywise.Enums;

namespace Staywise.Dtos;

public class BookingResponseDates
{
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
}