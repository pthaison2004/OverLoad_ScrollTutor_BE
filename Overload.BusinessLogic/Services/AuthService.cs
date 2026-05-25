using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Overload.BusinessLogic.Dtos;
using Overload.BusinessLogic.Helpers;
using Overload.BusinessLogic.Interfaces;
using Overload.DataAccessLayer.Entities;
using Overload.DataAccessLayer.Repositories;

namespace Overload.BusinessLogic.Services;

public class AuthService : IAuthService
{
    private readonly IRepository<User> _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IRepository<User> userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        // Check duplicate email
        var existingUsers = await _userRepository.FindAsync(u => u.Email == dto.Email.Trim().ToLower());
        if (existingUsers.Any())
        {
            throw new ArgumentException("Email is already registered.");
        }

        var now = DateTime.UtcNow;
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email.Trim().ToLower(),
            PasswordHash = PasswordHasher.HashPassword(dto.Password),
            FullName = dto.FullName.Trim(),
            Role = "student", // default role
            IsVerified = false,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _userRepository.AddAsync(newUser);
        await _userRepository.SaveAsync();

        var token = GenerateToken(newUser);

        return new AuthResponseDto
        {
            Token = token,
            UserId = newUser.Id,
            Email = newUser.Email,
            FullName = newUser.FullName ?? "",
            Role = newUser.Role ?? "student"
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLower();
        var users = await _userRepository.FindAsync(u => u.Email == email);
        var user = users.FirstOrDefault();

        if (user == null || !PasswordHasher.VerifyPassword(dto.Password, user.PasswordHash))
        {
            throw new ArgumentException("Invalid email or password.");
        }

        var token = GenerateToken(user);

        return new AuthResponseDto
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName ?? "",
            Role = user.Role ?? "student"
        };
    }

    private string GenerateToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Secret Key is not configured.")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role ?? "student"),
            new Claim(ClaimTypes.Name, user.FullName ?? "")
        };

        var durationInMinutes = double.Parse(jwtSettings["DurationInMinutes"] ?? "60");
        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(durationInMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
