using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StallosDotnetPleno.Application.Interfaces;
using StallosDotnetPleno.Application.Services;
using StallosDotnetPleno.Domain.ViewModels;

namespace StallosDotnetPleno.Api.Controllers;


[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;

    public AuthController(ITokenService tokenService, IUserService userService)
    {
        _tokenService = tokenService;
        _userService = userService;
    }

    [HttpPost("token")]
    [Authorize(AuthenticationSchemes = "Basic")]
    public IActionResult GenerateToken()
    {
        var username = HttpContext.User.Identity.Name;
        if (username != null && HttpContext.User.Identity.IsAuthenticated)
        {
            var token = _tokenService.GenerateToken(username);
            return Ok(new { token });
        }
        return Unauthorized();
    }
}