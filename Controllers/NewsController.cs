using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewRentalCarManagerAPI.Application.Features.News;
using NewRentalCarManagerAPI.Common;

namespace NewRentalCarManagerAPI.Controllers;

[ApiController]
[Route("api/news")]
public class NewsController : ControllerBase
{
    private readonly INewsService _service;
    public NewsController(INewsService service) => _service = service;

    /// <summary>Public: list approved articles</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetApproved()
    {
        var result = await _service.GetApprovedAsync();
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Admin: list all articles</summary>
    [HttpGet("all")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Owner / Admin: list my articles</summary>
    [HttpGet("mine")]
    [Authorize]
    public async Task<IActionResult> GetMine()
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized(DataResult.ResultError(401, "Không xác định được người dùng"));
        var result = await _service.GetByAuthorAsync(userId.Value);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Owner / Admin: create article (starts as pending)</summary>
    [HttpPost]
    [Authorize(Roles = "owner,admin")]
    public async Task<IActionResult> Create(CreateNewsArticleDto dto)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized(DataResult.ResultError(401, "Không xác định được người dùng"));
        var result = await _service.CreateAsync(userId.Value, dto);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Owner (own article) / Admin: update article</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "owner,admin")]
    public async Task<IActionResult> Update(Guid id, UpdateNewsArticleDto dto)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized(DataResult.ResultError(401, "Không xác định được người dùng"));
        var isAdmin = User.IsInRole("admin");
        var result = await _service.UpdateAsync(id, userId.Value, isAdmin, dto);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Admin: approve or reject an article</summary>
    [HttpPatch("{id:guid}/review")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Review(Guid id, ReviewNewsArticleDto dto)
    {
        var result = await _service.ReviewAsync(id, dto);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Owner (own) / Admin: delete article</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "owner,admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized(DataResult.ResultError(401, "Không xác định được người dùng"));
        var isAdmin = User.IsInRole("admin");
        var result = await _service.DeleteAsync(id, userId.Value, isAdmin);
        return StatusCode(result.StatusCode, result);
    }

    private Guid? GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(value, out var id) ? id : null;
    }
}
