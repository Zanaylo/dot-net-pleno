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
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ITokenService tokenService, ILogger<AuthController> logger)
    {
        _tokenService = tokenService;
        _logger = logger;
    }

    [HttpPost("token")]
    [Authorize(AuthenticationSchemes = "Basic")]
    public IActionResult GenerateToken()
    {
        try
        {
            string username = HttpContext.User.Identity!.Name;
            if (username != null && HttpContext.User.Identity.IsAuthenticated)
            {
                string token = _tokenService.GenerateToken(username);
                return Ok(new { token });
            }
            return Unauthorized();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error generating token for user: {HttpContext.User.Identity!.Name}");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }
}