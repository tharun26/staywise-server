using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

using Staywise.Data;
using Staywise.Dtos;
using Staywise.Models;

using AutoMapper;

namespace Staywise.Controllers;


[ApiController]
[Route("/api/listing")]
public class ListingController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;

    public ListingController(AppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateListingDto createListingDto)
    {
        var email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null)
        {
            return NotFound("User does not exist");
        }
        var listing = _mapper.Map<Listing>(createListingDto);
        listing.HostId = user.Id;


        _dbContext.Listings.Add(listing);
        await _dbContext.SaveChangesAsync();

        var response = _mapper.Map<ListingResponseDto>(listing);
        return Ok(response);
    } 

    [HttpGet("{id}")]
    public async Task<ActionResult<ListingResponseDto>> GetById(Guid id)
    {
        var listing = await _dbContext.Listings.FindAsync(id);
        if (listing == null) return NotFound();

        var response = _mapper.Map<ListingResponseDto>(listing);
        return Ok(response);
    }  
}
