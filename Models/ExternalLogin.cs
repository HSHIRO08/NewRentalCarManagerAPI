using System;
using System.Collections.Generic;
using NewRentalCarManagerAPI.Enums;

namespace NewRentalCarManagerAPI.Models;

public partial class ExternalLogin
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string ProviderUid { get; set; } = null!;

    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? TokenExpiresAt { get; set; }

    public string? ProfileData { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ProviderName Provider { get; set; }

    public virtual User User { get; set; } = null!;
}
