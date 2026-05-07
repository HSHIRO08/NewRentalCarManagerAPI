using System;

namespace NewRentalCarManagerAPI.Models;

public class NewsArticle
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string Summary { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string? Category { get; set; }

    public string? ImageUrl { get; set; }

    public Guid AuthorId { get; set; }

    /// <summary>pending | approved | rejected</summary>
    public string Status { get; set; } = "pending";

    public string? RejectReason { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual User Author { get; set; } = null!;
}
