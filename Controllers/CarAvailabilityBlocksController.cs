using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewRentalCarManagerAPI.Application.Features.Fleet;
using NewRentalCarManagerAPI.Common;

namespace NewRentalCarManagerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin,owner")]
public class CarAvailabilityBlocksController : ControllerBase
{
    private readonly ICarAvailabilityBlockService _service;
    public CarAvailabilityBlocksController(ICarAvailabilityBlockService service) => _service = service;

    [HttpGet("car/{carId:guid}")]
    public async Task<IActionResult> GetByCar(Guid carId)
        => Ok(ApiResult<IEnumerable<CarAvailabilityBlockDto>>.Ok(await _service.GetByCarAsync(carId)));

    [HttpPost]
    public async Task<IActionResult> Create(CreateCarAvailabilityBlockDto dto)
        => Ok(ApiResult<CarAvailabilityBlockDto>.Ok(await _service.CreateAsync(dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => await _service.DeleteAsync(id) ? Ok(ApiResult<bool>.Ok(true)) : NotFound(ApiResult<bool>.Fail("Not found"));
}
