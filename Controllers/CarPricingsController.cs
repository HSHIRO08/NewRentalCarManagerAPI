using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewRentalCarManagerAPI.Application.Features.Fleet;
using NewRentalCarManagerAPI.Common;

namespace NewRentalCarManagerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin,owner")]
public class CarPricingsController : ControllerBase
{
    private readonly ICarPricingService _service;
    public CarPricingsController(ICarPricingService service) => _service = service;

    [HttpGet("car/{carId:guid}")]
    public async Task<IActionResult> GetByCar(Guid carId)
        => Ok(ApiResult<IEnumerable<CarPricingDto>>.Ok(await _service.GetByCarAsync(carId)));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var r = await _service.GetByIdAsync(id);
        return r is null ? NotFound(ApiResult<CarPricingDto>.Fail("Not found")) : Ok(ApiResult<CarPricingDto>.Ok(r));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCarPricingDto dto)
    {
        var r = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = r.Id }, ApiResult<CarPricingDto>.Ok(r));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateCarPricingDto dto)
    {
        var r = await _service.UpdateAsync(id, dto);
        return r is null ? NotFound(ApiResult<CarPricingDto>.Fail("Not found")) : Ok(ApiResult<CarPricingDto>.Ok(r));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => await _service.DeleteAsync(id) ? Ok(ApiResult<bool>.Ok(true)) : NotFound(ApiResult<bool>.Fail("Not found"));
}
