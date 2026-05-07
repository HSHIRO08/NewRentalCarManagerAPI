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
        => Ok(ApiResult<IEnumerable<NewsArticleDto>>.Ok(await _service.GetApprovedAsync()));

    /// <summary>Admin: list all articles</summary>
    [HttpGet("all")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetAll()
        => Ok(ApiResult<IEnumerable<NewsArticleDto>>.Ok(await _service.GetAllAsync()));

    /// <summary>Owner / Admin: list my articles</summary>
    [HttpGet("mine")]
    [Authorize]
    public async Task<IActionResult> GetMine()
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized(ApiResult<IEnumerable<NewsArticleDto>>.Fail("Không xác định được người dùng"));
        return Ok(ApiResult<IEnumerable<NewsArticleDto>>.Ok(await _service.GetByAuthorAsync(userId.Value)));
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound(ApiResult<NewsArticleDto>.Fail("Không tìm thấy bài viết")) : Ok(ApiResult<NewsArticleDto>.Ok(result));
    }

    /// <summary>Owner / Admin: create article (starts as pending)</summary>
    [HttpPost]
    [Authorize(Roles = "owner,admin")]
    public async Task<IActionResult> Create(CreateNewsArticleDto dto)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized(ApiResult<NewsArticleDto>.Fail("Không xác định được người dùng"));
        var result = await _service.CreateAsync(userId.Value, dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResult<NewsArticleDto>.Ok(result));
    }

    /// <summary>Owner (own article) / Admin: update article</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "owner,admin")]
    public async Task<IActionResult> Update(Guid id, UpdateNewsArticleDto dto)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized(ApiResult<NewsArticleDto>.Fail("Không xác định được người dùng"));
        var isAdmin = User.IsInRole("admin");
        var result = await _service.UpdateAsync(id, userId.Value, isAdmin, dto);
        return result is null
            ? NotFound(ApiResult<NewsArticleDto>.Fail("Không tìm thấy hoặc không có quyền"))
            : Ok(ApiResult<NewsArticleDto>.Ok(result));
    }

    /// <summary>Admin: approve or reject an article</summary>
    [HttpPatch("{id:guid}/review")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Review(Guid id, ReviewNewsArticleDto dto)
    {
        var result = await _service.ReviewAsync(id, dto);
        return result is null
            ? NotFound(ApiResult<NewsArticleDto>.Fail("Không tìm thấy bài viết"))
            : Ok(ApiResult<NewsArticleDto>.Ok(result));
    }

    /// <summary>Owner (own) / Admin: delete article</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "owner,admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized(ApiResult<bool>.Fail("Không xác định được người dùng"));
        var isAdmin = User.IsInRole("admin");
        var deleted = await _service.DeleteAsync(id, userId.Value, isAdmin);
        return deleted ? Ok(ApiResult<bool>.Ok(true)) : NotFound(ApiResult<bool>.Fail("Không tìm thấy hoặc không có quyền"));
    }

    private Guid? GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(value, out var id) ? id : null;
    }
}
