using System.ComponentModel.DataAnnotations;

namespace OnlineCourses.Application.Dtos;

public record LoginDto([Required] string Email, [Required] string Password);
public record RegisterDto([Required] string Email, [Required] string Password, string? FullName);
public record AuthResponseDto(string Token);
