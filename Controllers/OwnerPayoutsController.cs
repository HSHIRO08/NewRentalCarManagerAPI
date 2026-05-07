using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewRentalCarManagerAPI.Application.Features.Payments;

namespace NewRentalCarManagerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class OwnerPayoutsController : ControllerBase
{
    private readonly IOwnerPayoutService _service;
    public OwnerPayoutsController(IOwnerPayoutService service) => _service = service;

    [HttpGet("owner/{ownerId:guid}")]
    public async Task<IActionResult> GetByOwner(Guid ownerId, [FromQuery] PaymentListInput input)
    {
        var result = await _service.GetByOwnerAsync(ownerId, input);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOwnerPayoutDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return StatusCode(result.StatusCode, result);
    }
}
