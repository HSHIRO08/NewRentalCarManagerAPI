using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewRentalCarManagerAPI.Application.Features.Fleet;
using NewRentalCarManagerAPI.Common;

namespace NewRentalCarManagerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CarModelsController : ControllerBase
{
    private readonly ICarModelService _service;
    public CarModelsController(ICarModelService service) => _service = service;

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(ApiResult<IEnumerable<CarModelDto>>.Ok(await _service.GetAllAsync()));

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var r = await _service.GetByIdAsync(id);
        return r is null ? NotFound(ApiResult<CarModelDto>.Fail("Not found")) : Ok(ApiResult<CarModelDto>.Ok(r));
    }

    [AllowAnonymous]
    [HttpGet("brand/{brandId:guid}")]
    public async Task<IActionResult> GetByBrand(Guid brandId)
        => Ok(ApiResult<IEnumerable<CarModelDto>>.Ok(await _service.GetByBrandAsync(brandId)));

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateCarModelDto dto)
    {
        var r = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = r.Id }, ApiResult<CarModelDto>.Ok(r));
    }

    [Authorize(Roles = "admin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateCarModelDto dto)
    {
        var r = await _service.UpdateAsync(id, dto);
        return r is null ? NotFound(ApiResult<CarModelDto>.Fail("Not found")) : Ok(ApiResult<CarModelDto>.Ok(r));
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => await _service.DeleteAsync(id) ? Ok(ApiResult<bool>.Ok(true)) : NotFound(ApiResult<bool>.Fail("Not found"));
}
