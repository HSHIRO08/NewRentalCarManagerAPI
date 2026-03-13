using System;
using System.Collections.Generic;

namespace NewRentalCarManagerAPI.Models;

public partial class ApiKey
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Name { get; set; } = null!;

    public string KeyHash { get; set; } = null!;

    public string KeyPrefix { get; set; } = null!;

    public List<string> Scopes { get; set; } = null!;

    public DateTime? ExpiresAt { get; set; }

    public DateTime? LastUsedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
