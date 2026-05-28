using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewRentalCarManagerAPI.Application.Features.Bookings;
using NewRentalCarManagerAPI.Common;

namespace NewRentalCarManagerAPI.Controllers;

[ApiController]
[Route("api/bookings/{bookingId:guid}/handover")]
[Authorize]
public class HandoverController : ControllerBase
{
    private readonly IHandoverService _service;
    public HandoverController(IHandoverService service) => _service = service;

    private Guid? GetActorId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? User.FindFirstValue("sub");
        return Guid.TryParse(raw, out var id) ? id : null;
    }

    /// <summary>Lấy danh sách handover records của một booking</summary>
    [HttpGet]
    public async Task<IActionResult> GetByBooking(Guid bookingId)
    {
        var result = await _service.GetByBookingAsync(bookingId);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Chủ xe / admin xác nhận bàn giao xe ra (Confirmed → Active)</summary>
    [HttpPost("checkin")]
    public async Task<IActionResult> CheckIn(Guid bookingId, CheckInDto dto)
    {
        var actorId = GetActorId();
        if (actorId is null) return Unauthorized(DataResult.ResultError(401, "Cannot identify user"));
        var result = await _service.CheckInAsync(bookingId, actorId.Value, dto);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Chủ xe / admin xác nhận trả xe (Active → Completed) — tính phí trễ hạn nếu có</summary>
    [HttpPost("checkout")]
    public async Task<IActionResult> CheckOut(Guid bookingId, CheckOutDto dto)
    {
        var actorId = GetActorId();
        if (actorId is null) return Unauthorized(DataResult.ResultError(401, "Cannot identify user"));
        var result = await _service.CheckOutAsync(bookingId, actorId.Value, dto);
        return StatusCode(result.StatusCode, result);
    }
}
