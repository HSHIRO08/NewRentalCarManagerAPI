using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewRentalCarManagerAPI.Application.Features.Fleet;
using NewRentalCarManagerAPI.Common;

namespace NewRentalCarManagerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CarBrandsController : ControllerBase
{
    private readonly ICarBrandService _service;
    public CarBrandsController(ICarBrandService service) => _service = service;

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(ApiResult<IEnumerable<CarBrandDto>>.Ok(await _service.GetAllAsync()));

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var r = await _service.GetByIdAsync(id);
        return r is null ? NotFound(ApiResult<CarBrandDto>.Fail("Not found")) : Ok(ApiResult<CarBrandDto>.Ok(r));
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateCarBrandDto dto)
    {
        var r = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = r.Id }, ApiResult<CarBrandDto>.Ok(r));
    }

    [Authorize(Roles = "admin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateCarBrandDto dto)
    {
        var r = await _service.UpdateAsync(id, dto);
        return r is null ? NotFound(ApiResult<CarBrandDto>.Fail("Not found")) : Ok(ApiResult<CarBrandDto>.Ok(r));
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => await _service.DeleteAsync(id) ? Ok(ApiResult<bool>.Ok(true)) : NotFound(ApiResult<bool>.Fail("Not found"));
}
