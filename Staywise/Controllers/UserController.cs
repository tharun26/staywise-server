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
public class UserController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;
    public UserController(AppDbContext context, IConfiguration configuration)
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
        return Ok(jwtToken);
    }

    [Authorize]
    [HttpPost("verify")]
    public async Task<IActionResult> Verify()
    {
        var email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Console.WriteLine("******** Email is {0}",email);

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