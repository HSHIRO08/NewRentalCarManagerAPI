using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewRentalCarManagerAPI.Application.Features.Fleet;
using NewRentalCarManagerAPI.Common;
using NewRentalCarManagerAPI.Infrastructure.Authorization;

namespace NewRentalCarManagerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CarsController : ControllerBase
{
    private readonly ICarService _service;
    public CarsController(ICarService service) => _service = service;

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] FleetListInput input)
    {
        var result = await _service.GetAllAsync(input);
        return StatusCode(result.StatusCode, result);
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [AllowAnonymous]
    [HttpGet("owner/{ownerId:guid}")]
    public async Task<IActionResult> GetByOwner(Guid ownerId, [FromQuery] FleetListInput input)
    {
        var result = await _service.GetByOwnerAsync(ownerId, input);
        return StatusCode(result.StatusCode, result);
    }

    [HasPermission("cars", "create")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateCarDto dto)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(DataResult.ResultError(401, "Invalid user ID"));
        dto.OwnerId = userId;
        var result = await _service.CreateAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HasPermission("cars", "update")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateCarDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return StatusCode(result.StatusCode, result);
    }

    [HasPermission("cars", "update")]
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> PatchStatus(Guid id, UpdateCarStatusDto dto)
    {
        var result = await _service.PatchStatusAsync(id, dto.Status);
        return StatusCode(result.StatusCode, result);
    }

    [HasPermission("cars", "delete")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        return StatusCode(result.StatusCode, result);
    }
}
