using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewRentalCarManagerAPI.Application.Features.Payments;
using NewRentalCarManagerAPI.Common;

namespace NewRentalCarManagerAPI.Controllers;

[ApiController]
[Route("api/bookings/{bookingId:guid}/deposit")]
[Authorize]
public class DepositsController : ControllerBase
{
    private readonly IDepositService _service;
    public DepositsController(IDepositService service) => _service = service;

    private Guid? GetActorId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? User.FindFirstValue("sub");
        return Guid.TryParse(raw, out var id) ? id : null;
    }

    /// <summary>Xem trạng thái deposit của booking</summary>
    [HttpGet]
    public async Task<IActionResult> GetStatus(Guid bookingId)
    {
        var result = await _service.GetStatusAsync(bookingId);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Thu deposit — booking phải ở trạng thái Confirmed</summary>
    [HttpPost("charge")]
    public async Task<IActionResult> Charge(Guid bookingId)
    {
        var actorId = GetActorId();
        if (actorId is null) return Unauthorized(DataResult.ResultError(401, "Cannot identify user"));
        var result = await _service.ChargeAsync(bookingId, actorId.Value);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Hoàn deposit — admin only, booking phải Completed/Cancelled</summary>
    [Authorize(Roles = "admin")]
    [HttpPost("refund")]
    public async Task<IActionResult> Refund(Guid bookingId, RefundDepositDto dto)
    {
        var actorId = GetActorId();
        if (actorId is null) return Unauthorized(DataResult.ResultError(401, "Cannot identify user"));
        var result = await _service.RefundAsync(bookingId, actorId.Value, dto);
        return StatusCode(result.StatusCode, result);
    }
}
