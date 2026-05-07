using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewRentalCarManagerAPI.Application.Features.Bookings;
using NewRentalCarManagerAPI.Common;

namespace NewRentalCarManagerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _service;
    public BookingsController(IBookingService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] BookingListInput input)
    {
        var result = await _service.GetAllAsync(input);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMine([FromQuery] BookingListInput input)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var renterId))
            return Unauthorized(DataResult.ResultError(401, "Cannot identify user"));

        var result = await _service.GetByRenterAsync(renterId, input);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("renter/{renterId:guid}")]
    public async Task<IActionResult> GetByRenter(Guid renterId, [FromQuery] BookingListInput input)
    {
        var result = await _service.GetByRenterAsync(renterId, input);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("car/{carId:guid}")]
    public async Task<IActionResult> GetByCar(Guid carId, [FromQuery] BookingListInput input)
    {
        var result = await _service.GetByCarAsync(carId, input);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBookingDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("quick")]
    public async Task<IActionResult> QuickCreate(QuickCreateBookingDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var renterId))
            return Unauthorized(DataResult.ResultError(401, "Cannot identify user"));

        var result = await _service.QuickCreateAsync(renterId, dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateBookingDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("{id:guid}/pay")]
    public async Task<IActionResult> PayNow(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var payerId))
            return Unauthorized(DataResult.ResultError(401, "Cannot identify user"));

        var result = await _service.PayBookingAsync(id, payerId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("{id:guid}/send-email")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> SendEmail(Guid id)
    {
        var result = await _service.SendEmailAsync(id);
        return StatusCode(result.StatusCode, result);
    }
}
