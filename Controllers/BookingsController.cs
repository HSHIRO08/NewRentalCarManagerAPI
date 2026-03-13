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

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateBookingDto dto)
    {
        var r = await _service.UpdateAsync(id, dto);
        return r is null ? NotFound(ApiResult<BookingDto>.Fail("Not found")) : Ok(ApiResult<BookingDto>.Ok(r));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => await _service.DeleteAsync(id) ? Ok(ApiResult<bool>.Ok(true)) : NotFound(ApiResult<bool>.Fail("Not found"));
}
