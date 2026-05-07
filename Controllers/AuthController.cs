using Microsoft.AspNetCore.Mvc;
using NewRentalCarManagerAPI.Application.Features.Auth;
using NewRentalCarManagerAPI.Common;

namespace NewRentalCarManagerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;
    public AuthController(IAuthService service) => _service = service;

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = DataResult.ResultSuccess(await _service.LoginAsync(dto), "Get success!");
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = DataResult.ResultSuccess(await _service.RegisterAsync(dto), "Insert success!", statusCode: 201);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequestDto dto)
    {
        var result = DataResult.ResultSuccess(await _service.RefreshAsync(dto), "Get success!");
        return StatusCode(result.StatusCode, result);
    }
}
