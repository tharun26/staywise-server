using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Staywise.Data;
using Staywise.Dtos;
using Staywise.Models;
namespace Staywise.Controllers;


[ApiController]
[Route("/api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;
    public BookingController(AppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<BookingResponseDto>> Create(CreateBookingDto createBookingDto)
    {
        var booking = _mapper.Map<Booking>(createBookingDto);
        booking.Status = Enums.Status.Confirmed;

        booking.CheckIn = DateTime.SpecifyKind(booking.CheckIn.Date, DateTimeKind.Utc);
        booking.CheckOut = DateTime.SpecifyKind(booking.CheckOut.Date, DateTimeKind.Utc);

        bool hasConflict = await _dbContext.Bookings.AnyAsync(b =>
                                                b.ListingId == booking.ListingId &&
                                                booking.CheckIn < b.CheckOut &&
                                                booking.CheckOut > b.CheckIn);

        if (hasConflict)
        {
            return BadRequest("This listing is already booked for the selected dates.");
        }

        _dbContext.Add(booking);
        await _dbContext.SaveChangesAsync();
        var response = _mapper.Map<BookingResponseDto>(booking);
        return Ok(response);
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<BookingResponseDto>>> GetAll()
    {
        var bookings = await _dbContext.Bookings.ToListAsync();
        var response = _mapper.Map<List<BookingResponseDto>>(bookings);
        return Ok(response);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<List<BookingResponseDto>>> GetById(Guid Id)
    {
        var booking = await _dbContext.Bookings.FirstOrDefaultAsync(b => b.Id == Id);
        if (booking is null)
        {
            return NotFound("Booking Id Does not exist");
        }
        var response = _mapper.Map<BookingResponseDto>(booking);
        return Ok(response);
    }

    [Authorize]
    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelBooking(Guid Id)
    {
        var booking = await _dbContext.Bookings.FirstOrDefaultAsync(b => b.Id == Id);
        if (booking is null)
        {
            return NotFound("Booking Id does not exist");
        }
        booking.Status = Enums.Status.Cancelled;
        await _dbContext.SaveChangesAsync();
        return Ok();
    }

}