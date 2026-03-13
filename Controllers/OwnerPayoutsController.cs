using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewRentalCarManagerAPI.Application.Features.Payments;
using NewRentalCarManagerAPI.Common;

namespace NewRentalCarManagerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class OwnerPayoutsController : ControllerBase
{
    private readonly IOwnerPayoutService _service;
    public OwnerPayoutsController(IOwnerPayoutService service) => _service = service;

    [HttpGet("owner/{ownerId:guid}")]
    public async Task<IActionResult> GetByOwner(Guid ownerId)
        => Ok(ApiResult<IEnumerable<OwnerPayoutDto>>.Ok(await _service.GetByOwnerAsync(ownerId)));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var r = await _service.GetByIdAsync(id);
        return r is null ? NotFound(ApiResult<OwnerPayoutDto>.Fail("Not found")) : Ok(ApiResult<OwnerPayoutDto>.Ok(r));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOwnerPayoutDto dto)
    {
        var r = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = r.Id }, ApiResult<OwnerPayoutDto>.Ok(r));
    }
}
