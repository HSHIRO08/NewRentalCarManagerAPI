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
        var result = await _service.LoginAsync(dto);
        return Ok(ApiResult<TokenDto>.Ok(result));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await _service.RegisterAsync(dto);
        return Ok(ApiResult<TokenDto>.Ok(result));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequestDto dto)
    {
        var result = await _service.RefreshAsync(dto);
        return Ok(ApiResult<TokenDto>.Ok(result));
    }
}
