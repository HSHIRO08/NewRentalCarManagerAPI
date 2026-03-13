using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewRentalCarManagerAPI.Application.Features.Ops;
using NewRentalCarManagerAPI.Common;

namespace NewRentalCarManagerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _service;
    public ReviewsController(IReviewService service) => _service = service;

    [AllowAnonymous]
    [HttpGet("booking/{bookingId:guid}")]
    public async Task<IActionResult> GetByBooking(Guid bookingId)
        => Ok(ApiResult<IEnumerable<ReviewDto>>.Ok(await _service.GetByBookingAsync(bookingId)));

    [AllowAnonymous]
    [HttpGet("car/{carId:guid}")]
    public async Task<IActionResult> GetByCar(Guid carId)
        => Ok(ApiResult<IEnumerable<ReviewDto>>.Ok(await _service.GetByCarAsync(carId)));

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var r = await _service.GetByIdAsync(id);
        return r is null ? NotFound(ApiResult<ReviewDto>.Fail("Not found")) : Ok(ApiResult<ReviewDto>.Ok(r));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateReviewDto dto)
    {
        var r = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = r.Id }, ApiResult<ReviewDto>.Ok(r));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => await _service.DeleteAsync(id) ? Ok(ApiResult<bool>.Ok(true)) : NotFound(ApiResult<bool>.Fail("Not found"));
}
