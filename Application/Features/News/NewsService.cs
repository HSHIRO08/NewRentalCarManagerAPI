using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewRentalCarManagerAPI.Common;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Models;
using System.Net;

namespace NewRentalCarManagerAPI.Application.Features.News;

public interface INewsService
{
    Task<DataResult> GetAllAsync();
    Task<DataResult> GetApprovedAsync();
    Task<DataResult> GetByAuthorAsync(Guid authorId);
    Task<DataResult> GetByIdAsync(Guid id);
    Task<DataResult> CreateAsync(Guid authorId, CreateNewsArticleDto dto);
    Task<DataResult> UpdateAsync(Guid id, Guid requesterId, bool isAdmin, UpdateNewsArticleDto dto);
    Task<DataResult> ReviewAsync(Guid id, ReviewNewsArticleDto dto);
    Task<DataResult> DeleteAsync(Guid id, Guid requesterId, bool isAdmin);
}

public class NewsService : INewsService
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<NewsService> _logger;

    public NewsService(IUnitOfWork uow, ILogger<NewsService> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<DataResult> GetAllAsync()
    {
        try
        {
            var articles = await _uow.NewsArticles.Query()
                .Include(a => a.Author)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
            return DataResult.ResultSuccess(articles.Select(MapToDto).ToList(), "Get success!", articles.Count);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get all news articles");
            throw;
        }
    }

    public async Task<DataResult> GetApprovedAsync()
    {
        try
        {
            var articles = await _uow.NewsArticles.Query()
                .Include(a => a.Author)
                .Where(a => a.Status == "approved")
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
            return DataResult.ResultSuccess(articles.Select(MapToDto).ToList(), "Get success!", articles.Count);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get approved news");
            throw;
        }
    }

    public async Task<DataResult> GetByAuthorAsync(Guid authorId)
    {
        try
        {
            var articles = await _uow.NewsArticles.Query()
                .Include(a => a.Author)
                .Where(a => a.AuthorId == authorId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
            return DataResult.ResultSuccess(articles.Select(MapToDto).ToList(), "Get success!", articles.Count);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get news by author {AuthorId}", authorId);
            throw;
        }
    }

    public async Task<DataResult> GetByIdAsync(Guid id)
    {
        try
        {
            var article = await _uow.NewsArticles.Query()
                .Include(a => a.Author)
                .FirstOrDefaultAsync(a => a.Id == id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "News article not found!");
            return DataResult.ResultSuccess(MapToDto(article), "Get success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get news article {Id}", id);
            throw;
        }
    }

    public async Task<DataResult> CreateAsync(Guid authorId, CreateNewsArticleDto dto)
    {
        try
        {
            var article = new NewsArticle
            {
                Id = Guid.NewGuid(),
                AuthorId = authorId,
                Title = dto.Title,
                Summary = dto.Summary,
                Content = dto.Content,
                Category = dto.Category,
                ImageUrl = dto.ImageUrl,
                Status = "pending",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            await _uow.NewsArticles.AddAsync(article);

            var saved = await _uow.NewsArticles.Query()
                .Include(a => a.Author)
                .FirstOrDefaultAsync(a => a.Id == article.Id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.InternalServerError, "Create news failed.");
            return DataResult.ResultSuccess(MapToDto(saved), "Insert success!", statusCode: 201);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create news article");
            throw;
        }
    }

    public async Task<DataResult> UpdateAsync(Guid id, Guid requesterId, bool isAdmin, UpdateNewsArticleDto dto)
    {
        try
        {
            var article = await _uow.NewsArticles.Query()
                .Include(a => a.Author)
                .FirstOrDefaultAsync(a => a.Id == id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "News article not found!");
            if (!isAdmin && article.AuthorId != requesterId)
            {
                throw new UserFriendlyException((int)HttpStatusCode.Forbidden, "You do not have permission to update this article.");
            }

            if (dto.Title is not null) article.Title = dto.Title;
            if (dto.Summary is not null) article.Summary = dto.Summary;
            if (dto.Content is not null) article.Content = dto.Content;
            if (dto.Category is not null) article.Category = dto.Category;
            if (dto.ImageUrl is not null) article.ImageUrl = dto.ImageUrl;
            article.UpdatedAt = DateTime.UtcNow;

            return DataResult.ResultSuccess(MapToDto(article), "Update success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to update news article {Id}", id);
            throw;
        }
    }

    public async Task<DataResult> ReviewAsync(Guid id, ReviewNewsArticleDto dto)
    {
        try
        {
            var article = await _uow.NewsArticles.Query()
                .Include(a => a.Author)
                .FirstOrDefaultAsync(a => a.Id == id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "News article not found!");

            var action = dto.Action.Trim().ToLowerInvariant();
            if (action != "approved" && action != "rejected")
            {
                throw new UserFriendlyException((int)HttpStatusCode.BadRequest, "Action must be 'approved' or 'rejected'.");
            }

            article.Status = action == "approved" ? "approved" : "rejected";
            article.RejectReason = action == "rejected" ? dto.RejectReason : null;
            article.UpdatedAt = DateTime.UtcNow;

            return DataResult.ResultSuccess(MapToDto(article), "Review success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to review news article {Id}", id);
            throw;
        }
    }

    public async Task<DataResult> DeleteAsync(Guid id, Guid requesterId, bool isAdmin)
    {
        try
        {
            var article = await _uow.NewsArticles.Query()
                .FirstOrDefaultAsync(a => a.Id == id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "News article not found!");
            if (!isAdmin && article.AuthorId != requesterId)
            {
                throw new UserFriendlyException((int)HttpStatusCode.Forbidden, "You do not have permission to delete this article.");
            }

            _uow.NewsArticles.Remove(article);
            return DataResult.ResultSuccess(true, "Delete success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to delete news article {Id}", id);
            throw;
        }
    }

    private static NewsArticleDto MapToDto(NewsArticle a) => new()
    {
        Id = a.Id,
        Title = a.Title,
        Summary = a.Summary,
        Content = a.Content,
        Category = a.Category,
        ImageUrl = a.ImageUrl,
        AuthorId = a.AuthorId,
        AuthorName = a.Author?.FullName ?? "Ẩn danh",
        Status = a.Status,
        RejectReason = a.RejectReason,
        CreatedAt = a.CreatedAt,
        UpdatedAt = a.UpdatedAt,
    };
}
