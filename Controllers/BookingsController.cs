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
    public async Task<IActionResult> GetAll()
        => Ok(ApiResult<IEnumerable<BookingDto>>.Ok(await _service.GetAllAsync()));

    [HttpGet("mine")]
    public async Task<IActionResult> GetMine()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var renterId))
            return Unauthorized(ApiResult<IEnumerable<BookingDto>>.Fail("Cannot identify user"));

        return Ok(ApiResult<IEnumerable<BookingDto>>.Ok(await _service.GetByRenterAsync(renterId)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var r = await _service.GetByIdAsync(id);
        return r is null ? NotFound(ApiResult<BookingDto>.Fail("Not found")) : Ok(ApiResult<BookingDto>.Ok(r));
    }

    [HttpGet("renter/{renterId:guid}")]
    public async Task<IActionResult> GetByRenter(Guid renterId)
        => Ok(ApiResult<IEnumerable<BookingDto>>.Ok(await _service.GetByRenterAsync(renterId)));

    [HttpGet("car/{carId:guid}")]
    public async Task<IActionResult> GetByCar(Guid carId)
        => Ok(ApiResult<IEnumerable<BookingDto>>.Ok(await _service.GetByCarAsync(carId)));

    [HttpPost]
    public async Task<IActionResult> Create(CreateBookingDto dto)
    {
        var r = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = r.Id }, ApiResult<BookingDto>.Ok(r));
    }

    [HttpPost("quick")]
    public async Task<IActionResult> QuickCreate(QuickCreateBookingDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var renterId))
            return Unauthorized(ApiResult<BookingDto>.Fail("Cannot identify user"));

        var r = await _service.QuickCreateAsync(renterId, dto);
        return CreatedAtAction(nameof(GetById), new { id = r.Id }, ApiResult<BookingDto>.Ok(r));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateBookingDto dto)
    {
        var r = await _service.UpdateAsync(id, dto);
        return r is null ? NotFound(ApiResult<BookingDto>.Fail("Not found")) : Ok(ApiResult<BookingDto>.Ok(r));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => await _service.DeleteAsync(id) ? Ok(ApiResult<bool>.Ok(true)) : NotFound(ApiResult<bool>.Fail("Not found"));

    [HttpPost("{id:guid}/pay")]
    public async Task<IActionResult> PayNow(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var payerId))
            return Unauthorized(ApiResult<BookingDto>.Fail("Cannot identify user"));

        var r = await _service.PayBookingAsync(id, payerId);
        return r is null ? NotFound(ApiResult<BookingDto>.Fail("Booking not found")) : Ok(ApiResult<BookingDto>.Ok(r));
    }
}
