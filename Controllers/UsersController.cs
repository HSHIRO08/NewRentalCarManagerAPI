using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewRentalCarManagerAPI.Application.Features.Users;
using NewRentalCarManagerAPI.Common;

namespace NewRentalCarManagerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;
    public UsersController(IUserService service) => _service = service;

    [Authorize(Roles = "admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(ApiResult<IEnumerable<UserDto>>.Ok(await _service.GetAllAsync()));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var r = await _service.GetByIdAsync(id);
        return r is null ? NotFound(ApiResult<UserDto>.Fail("Not found")) : Ok(ApiResult<UserDto>.Ok(r));
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateUserDto dto)
    {
        var r = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = r.Id }, ApiResult<UserDto>.Ok(r));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateUserDto dto)
    {
        var r = await _service.UpdateAsync(id, dto);
        return r is null ? NotFound(ApiResult<UserDto>.Fail("Not found")) : Ok(ApiResult<UserDto>.Ok(r));
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => await _service.DeleteAsync(id) ? Ok(ApiResult<bool>.Ok(true)) : NotFound(ApiResult<bool>.Fail("Not found"));
}
