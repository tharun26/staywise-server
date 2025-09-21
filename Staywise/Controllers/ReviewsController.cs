using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Staywise.Data;
using Staywise.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Staywise.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;

    public ReviewsController(AppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<ReviewResponseDto>>> GetAll()
    {
        var email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null)
        {
            return NotFound("User not found");
        }
        var reviews = await _dbContext.Reviews.Where(r => r.AuthorId == user.Id).ToListAsync();
        var response = _mapper.Map<List<ReviewResponseDto>>(reviews);
        return response;
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewResponseDto>> GetById(Guid Id)
    {
        var email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null)
        {
            return NotFound("User not found");
        }
        var review = await _dbContext.Reviews.FirstAsync(r => r.AuthorId == user.Id && r.Id == Id);
        var response = _mapper.Map<ReviewResponseDto>(review);
        return response;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ReviewResponseDto>> Create([FromBody]CreateReviewDto createReviewDto)
    {
        var email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null)
        {
            return NotFound("User not found");
        }

        var review = _mapper.Map<Review>(createReviewDto);
        review.AuthorId = user.Id;
        _dbContext.Add(review);

        await _dbContext.SaveChangesAsync();

        var response = _mapper.Map<ReviewResponseDto>(review);

        return Ok(response);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<ReviewResponseDto>> Update([FromBody]UpdateReviewDto updateReviewDto, Guid Id)
    {
        var email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null)
        {
            return NotFound("User not found");
        }

        var existingReview = await _dbContext.Reviews.FindAsync(Id);

        if (existingReview is null)
        {
            return NotFound("Review does not exist");
        }

        _mapper.Map(updateReviewDto, existingReview);

        _dbContext.Reviews.Update(existingReview);

        await _dbContext.SaveChangesAsync();
       
        var response = _mapper.Map<ReviewResponseDto>(existingReview);

        return Ok(response);
    }


}