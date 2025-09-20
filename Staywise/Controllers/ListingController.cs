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
[Route("/api/[controller]")]
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
    public async Task<ActionResult<ListingResponseDto>> Create([FromForm] CreateListingDto createListingDto)
    {
        var email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null)
        {
            return NotFound("User does not exist");
        }
        var listing = _mapper.Map<Listing>(createListingDto);
        listing.HostId = user.Id;

        if (createListingDto.Photo != null)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{createListingDto.Photo.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await createListingDto.Photo.CopyToAsync(stream);
            }

            // Save relative URL to DB
            listing.Photos = new List<string> { $"/uploads/{uniqueFileName}" };
        }


        _dbContext.Listings.Add(listing);
        await _dbContext.SaveChangesAsync();

        var response = _mapper.Map<ListingResponseDto>(listing);
        return Ok(response);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<ListingResponseDto>> Update([FromForm] UpdateListingDto updateListingDto, Guid Id)
    {
        if (Id != updateListingDto.Id)
        {
            return BadRequest("Id in payload and Id given in url is different");
        }

        var email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user is null)
        {
            return NotFound("User does not exist");
        }

        var existingListing = await _dbContext.Listings.FindAsync(Id);

        _mapper.Map(updateListingDto, existingListing);


        if (updateListingDto.Photo != null)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{updateListingDto.Photo.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await updateListingDto.Photo.CopyToAsync(stream);
            }

            // Save relative URL to DB
            existingListing.Photos = new List<string> { $"/uploads/{uniqueFileName}" };
        }

        existingListing.HostId = user.Id;

        _dbContext.Listings.Update(existingListing);
        await _dbContext.SaveChangesAsync();

        var response = _mapper.Map<ListingResponseDto>(existingListing);
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

    [HttpGet]
    public async Task<ActionResult<List<ListingResponseDto>>> Get()
    {
        var listings = await _dbContext.Listings.ToListAsync();
        var response = _mapper.Map<List<ListingResponseDto>>(listings);
        return Ok(response);
    }
}
