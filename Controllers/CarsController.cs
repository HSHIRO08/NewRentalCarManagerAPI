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
    public async Task<IActionResult> GetAll()
        => Ok(ApiResult<IEnumerable<CarDto>>.Ok(await _service.GetAllAsync()));

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var r = await _service.GetByIdAsync(id);
        return r is null ? NotFound(ApiResult<CarDto>.Fail("Not found")) : Ok(ApiResult<CarDto>.Ok(r));
    }

    [AllowAnonymous]
    [HttpGet("owner/{ownerId:guid}")]
    public async Task<IActionResult> GetByOwner(Guid ownerId)
        => Ok(ApiResult<IEnumerable<CarDto>>.Ok(await _service.GetByOwnerAsync(ownerId)));

    [HasPermission("cars", "create")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateCarDto dto)
    {
        var r = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = r.Id }, ApiResult<CarDto>.Ok(r));
    }

    [HasPermission("cars", "update")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateCarDto dto)
    {
        var r = await _service.UpdateAsync(id, dto);
        return r is null ? NotFound(ApiResult<CarDto>.Fail("Not found")) : Ok(ApiResult<CarDto>.Ok(r));
    }

    [HasPermission("cars", "delete")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => await _service.DeleteAsync(id) ? Ok(ApiResult<bool>.Ok(true)) : NotFound(ApiResult<bool>.Fail("Not found"));
}
