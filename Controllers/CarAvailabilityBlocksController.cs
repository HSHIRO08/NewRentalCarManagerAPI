using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewRentalCarManagerAPI.Application.Features.Fleet;

namespace NewRentalCarManagerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin,owner")]
public class CarAvailabilityBlocksController : ControllerBase
{
    private readonly ICarAvailabilityBlockService _service;
    public CarAvailabilityBlocksController(ICarAvailabilityBlockService service) => _service = service;

    [HttpGet("car/{carId:guid}")]
    public async Task<IActionResult> GetByCar(Guid carId, [FromQuery] FleetListInput input)
    {
        var result = await _service.GetByCarAsync(carId, input);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCarAvailabilityBlockDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        return StatusCode(result.StatusCode, result);
    }
}
