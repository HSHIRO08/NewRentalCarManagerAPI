using Microsoft.EntityFrameworkCore;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Models;

namespace NewRentalCarManagerAPI.Application.Features.News;

public interface INewsService
{
    Task<IEnumerable<NewsArticleDto>> GetAllAsync();
    Task<IEnumerable<NewsArticleDto>> GetApprovedAsync();
    Task<IEnumerable<NewsArticleDto>> GetByAuthorAsync(Guid authorId);
    Task<NewsArticleDto?> GetByIdAsync(Guid id);
    Task<NewsArticleDto> CreateAsync(Guid authorId, CreateNewsArticleDto dto);
    Task<NewsArticleDto?> UpdateAsync(Guid id, Guid requesterId, bool isAdmin, UpdateNewsArticleDto dto);
    Task<NewsArticleDto?> ReviewAsync(Guid id, ReviewNewsArticleDto dto);
    Task<bool> DeleteAsync(Guid id, Guid requesterId, bool isAdmin);
}

public class NewsService : INewsService
{
    private readonly IUnitOfWork _uow;

    public NewsService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<NewsArticleDto>> GetAllAsync()
    {
        var articles = await _uow.NewsArticles.Query()
            .Include(a => a.Author)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
        return articles.Select(MapToDto);
    }

    public async Task<IEnumerable<NewsArticleDto>> GetApprovedAsync()
    {
        var articles = await _uow.NewsArticles.Query()
            .Include(a => a.Author)
            .Where(a => a.Status == "approved")
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
        return articles.Select(MapToDto);
    }

    public async Task<IEnumerable<NewsArticleDto>> GetByAuthorAsync(Guid authorId)
    {
        var articles = await _uow.NewsArticles.Query()
            .Include(a => a.Author)
            .Where(a => a.AuthorId == authorId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
        return articles.Select(MapToDto);
    }

    public async Task<NewsArticleDto?> GetByIdAsync(Guid id)
    {
        var article = await _uow.NewsArticles.Query()
            .Include(a => a.Author)
            .FirstOrDefaultAsync(a => a.Id == id);
        return article is null ? null : MapToDto(article);
    }

    public async Task<NewsArticleDto> CreateAsync(Guid authorId, CreateNewsArticleDto dto)
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
        await _uow.SaveChangesAsync();

        var saved = await _uow.NewsArticles.Query()
            .Include(a => a.Author)
            .FirstAsync(a => a.Id == article.Id);
        return MapToDto(saved);
    }

    public async Task<NewsArticleDto?> UpdateAsync(Guid id, Guid requesterId, bool isAdmin, UpdateNewsArticleDto dto)
    {
        var article = await _uow.NewsArticles.Query()
            .Include(a => a.Author)
            .FirstOrDefaultAsync(a => a.Id == id);
        if (article is null) return null;
        if (!isAdmin && article.AuthorId != requesterId) return null;

        if (dto.Title is not null) article.Title = dto.Title;
        if (dto.Summary is not null) article.Summary = dto.Summary;
        if (dto.Content is not null) article.Content = dto.Content;
        if (dto.Category is not null) article.Category = dto.Category;
        if (dto.ImageUrl is not null) article.ImageUrl = dto.ImageUrl;
        article.UpdatedAt = DateTime.UtcNow;

        await _uow.SaveChangesAsync();
        return MapToDto(article);
    }

    public async Task<NewsArticleDto?> ReviewAsync(Guid id, ReviewNewsArticleDto dto)
    {
        var article = await _uow.NewsArticles.Query()
            .Include(a => a.Author)
            .FirstOrDefaultAsync(a => a.Id == id);
        if (article is null) return null;

        article.Status = dto.Action == "approved" ? "approved" : "rejected";
        article.RejectReason = dto.Action == "rejected" ? dto.RejectReason : null;
        article.UpdatedAt = DateTime.UtcNow;

        await _uow.SaveChangesAsync();
        return MapToDto(article);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid requesterId, bool isAdmin)
    {
        var article = await _uow.NewsArticles.Query()
            .FirstOrDefaultAsync(a => a.Id == id);
        if (article is null) return false;
        if (!isAdmin && article.AuthorId != requesterId) return false;

        _uow.NewsArticles.Remove(article);
        await _uow.SaveChangesAsync();
        return true;
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
