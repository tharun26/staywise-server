using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;
using Staywise.Data;
using Staywise.Dtos;
using Staywise.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Staywise.Controllers;

[ApiController]
[Route("/api")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;
    public UsersController(AppDbContext context, IConfiguration configuration)
    {
        _dbContext = context;
        _configuration = configuration;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] SignUpDto signUpDto)
    {
        if (await _dbContext.Users.AnyAsync(u => u.Email == signUpDto.Email))
        {
            return BadRequest("Email already in use");
        }

        var user = new User
        {
            Name = signUpDto.Name,
            Email = signUpDto.Email,
            PasswordHash = HashPassword(signUpDto.Password)
        };

        await _dbContext.AddAsync<User>(user);
        await _dbContext.SaveChangesAsync();

        return Ok(new { user.Id, user.Name, user.Email });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
        if (user == null)
        {
            return Unauthorized("Invalid Email or Password");
        }
        if (user.PasswordHash != HashPassword(loginDto.Password))
        {
            return Unauthorized("Invalid Email or Password");
        }
        /// generate jwt token
        var jwtToken = GenerateJwtToken(user);
        return Ok(new { authToken = jwtToken, user  });
    }

    [Authorize]
    [HttpPost("verify")]
    public async Task<IActionResult> Verify()
    {
        var email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (email == null)
        {
            return Unauthorized("No Claims Info");
        }
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) {
            return Unauthorized("User does not exist");
        }

        return Ok(new { user.Id, user.Email, user.Name });
    }

    [Authorize]
    [HttpPost("users/favorites/{id}")]
    public async Task<IActionResult> AddFav(Guid id)
    {
        var email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (email == null)
        {
            return Unauthorized("No Claims Info");
        }
        var user = await _dbContext.Users
            .Include(u => u.FavoriteListings)
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user is null)
        {
            return BadRequest("User does not exist");
        }

        var listing = _dbContext.Listings.Find(id);
        if (listing is null)
        {
            return BadRequest("Listing does not exist");
        }

        if (!user.FavoriteListings.Any(l => l.Id == id)) 
        {
            user.FavoriteListings.Add(listing);
            await _dbContext.SaveChangesAsync();
        }
        var favorites = user.FavoriteListings.Select(l => l.Id).ToList();

        return Ok(favorites);
    }

    [Authorize]
    [HttpGet("users/favorites")]
    public async Task<ActionResult<List<Guid>>> GetAllFav()
    {
        var email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (email == null)
        {
            return Unauthorized("No Claims Info");
        }
        var user = await _dbContext.Users.Include(u => u.FavoriteListings).FirstOrDefaultAsync(u => u.Email == email);

        if (user is null)
        {
            return BadRequest("User does not exist");
        }

        var favorites = user.FavoriteListings.Select(l => l.Id).ToList();
        return Ok(favorites);
    }

    [Authorize]
    [HttpDelete("users/favorites/{id}")]
    public async Task<ActionResult<List<Guid>>> DeleteFav(Guid id)
    {
        var email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (email == null)
        {
            return Unauthorized("No Claims Info");
        }
        var user = await _dbContext.Users.Include(u => u.FavoriteListings).FirstOrDefaultAsync(u => u.Email == email);

        if (user is null)
        {
            return BadRequest("User does not exist");
        }

        var listingToRemove = user.FavoriteListings.FirstOrDefault(l => l.Id == id);
        if (listingToRemove != null)
        {
            user.FavoriteListings.Remove(listingToRemove);
            await _dbContext.SaveChangesAsync();
        }
        var favorites = user.FavoriteListings.Select(l => l.Id).ToList();
        return Ok(favorites);
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
           claims: new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Email),
                new Claim(ClaimTypes.Name, user.Name)
            },
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);

    }
    private string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}