using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewRentalCarManagerAPI.Application.Features.Users;
using NewRentalCarManagerAPI.Common;

namespace NewRentalCarManagerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;
    public UsersController(IUserService service) => _service = service;

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(DataResult.ResultError(401, "Invalid user ID"));
        var result = await _service.GetByIdAsync(userId);
        return StatusCode(result.StatusCode, result);
    }

    [Authorize(Roles = "admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
        if (!User.IsInRole("admin") && id.ToString() != currentUserId)
            return Forbid();
        var result = await _service.GetByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateUserDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateUserDto dto)
    {
        var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
        if (!User.IsInRole("admin") && id.ToString() != currentUserId)
            return Forbid();
        var result = await _service.UpdateAsync(id, dto);
        return StatusCode(result.StatusCode, result);
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>User tự nộp CCCD + bằng lái — KYC status → Pending</summary>
    [HttpPatch("me/kyc")]
    public async Task<IActionResult> SubmitKyc(SubmitKycDto dto)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(DataResult.ResultError(401, "Invalid user ID"));
        var result = await _service.SubmitKycAsync(userId, dto);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Admin duyệt / từ chối KYC</summary>
    [Authorize(Roles = "admin")]
    [HttpPatch("{id:guid}/kyc/review")]
    public async Task<IActionResult> ReviewKyc(Guid id, ReviewKycDto dto)
    {
        var result = await _service.ReviewKycAsync(id, dto);
        return StatusCode(result.StatusCode, result);
    }
}
