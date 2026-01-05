using OnlineCourses.Application.Dtos;

namespace OnlineCourses.Application.Common.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto request);
    Task RegisterAsync(RegisterDto request);
}
