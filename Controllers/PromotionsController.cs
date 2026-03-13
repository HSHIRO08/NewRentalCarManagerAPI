using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewRentalCarManagerAPI.Application.Features.Bookings;
using NewRentalCarManagerAPI.Common;

namespace NewRentalCarManagerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PromotionsController : ControllerBase
{
    private readonly IPromotionService _service;
    public PromotionsController(IPromotionService service) => _service = service;

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(ApiResult<IEnumerable<PromotionDto>>.Ok(await _service.GetAllAsync()));

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var r = await _service.GetByIdAsync(id);
        return r is null ? NotFound(ApiResult<PromotionDto>.Fail("Not found")) : Ok(ApiResult<PromotionDto>.Ok(r));
    }

    [AllowAnonymous]
    [HttpGet("code/{code}")]
    public async Task<IActionResult> GetByCode(string code)
    {
        var r = await _service.GetByCodeAsync(code);
        return r is null ? NotFound(ApiResult<PromotionDto>.Fail("Not found")) : Ok(ApiResult<PromotionDto>.Ok(r));
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreatePromotionDto dto)
    {
        var r = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = r.Id }, ApiResult<PromotionDto>.Ok(r));
    }

    [Authorize(Roles = "admin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdatePromotionDto dto)
    {
        var r = await _service.UpdateAsync(id, dto);
        return r is null ? NotFound(ApiResult<PromotionDto>.Fail("Not found")) : Ok(ApiResult<PromotionDto>.Ok(r));
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => await _service.DeleteAsync(id) ? Ok(ApiResult<bool>.Ok(true)) : NotFound(ApiResult<bool>.Fail("Not found"));
}
