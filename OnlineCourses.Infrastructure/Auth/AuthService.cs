using Microsoft.AspNetCore.Identity;
using OnlineCourses.Application.Common.Interfaces;
using OnlineCourses.Application.Dtos;

namespace OnlineCourses.Infrastructure.Auth;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JwtProvider _jwtProvider;

    public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, JwtProvider jwtProvider)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtProvider = jwtProvider;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            throw new Exception("Invalid credentials");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
            throw new Exception("Invalid credentials");

        var token = _jwtProvider.Generate(user);
        return new AuthResponseDto(token);
    }

    public async Task RegisterAsync(RegisterDto request)
    {
        var user = new ApplicationUser 
        { 
            UserName = request.Email, 
            Email = request.Email,
            FullName = request.FullName
        };
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Registration failed: {errors}");
        }
    }
}
