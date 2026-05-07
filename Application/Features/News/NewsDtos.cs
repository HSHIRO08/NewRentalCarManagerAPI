namespace NewRentalCarManagerAPI.Application.Features.News;

// ───── DTOs ─────
public class NewsArticleDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Summary { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? Category { get; set; }
    public string? ImageUrl { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = null!;
    public string Status { get; set; } = "pending";
    public string? RejectReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateNewsArticleDto
{
    public string Title { get; set; } = null!;
    public string Summary { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? Category { get; set; }
    public string? ImageUrl { get; set; }
}

public class UpdateNewsArticleDto
{
    public string? Title { get; set; }
    public string? Summary { get; set; }
    public string? Content { get; set; }
    public string? Category { get; set; }
    public string? ImageUrl { get; set; }
}

public class ReviewNewsArticleDto
{
    /// <summary>approved | rejected</summary>
    public string Action { get; set; } = null!;
    public string? RejectReason { get; set; }
}
