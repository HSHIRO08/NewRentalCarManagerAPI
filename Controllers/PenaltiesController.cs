using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewRentalCarManagerAPI.Application.Features.Ops;
using NewRentalCarManagerAPI.Common;

namespace NewRentalCarManagerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class PenaltiesController : ControllerBase
{
    private readonly IPenaltyService _service;
    public PenaltiesController(IPenaltyService service) => _service = service;

    [HttpGet("booking/{bookingId:guid}")]
    public async Task<IActionResult> GetByBooking(Guid bookingId)
        => Ok(ApiResult<IEnumerable<PenaltyDto>>.Ok(await _service.GetByBookingAsync(bookingId)));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var r = await _service.GetByIdAsync(id);
        return r is null ? NotFound(ApiResult<PenaltyDto>.Fail("Not found")) : Ok(ApiResult<PenaltyDto>.Ok(r));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePenaltyDto dto)
    {
        var r = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = r.Id }, ApiResult<PenaltyDto>.Ok(r));
    }
}
