using System;
using System.Collections.Generic;
using NewRentalCarManagerAPI.Enums;

namespace NewRentalCarManagerAPI.Models;

public partial class NotificationLog
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string EventType { get; set; } = null!;

    public string Recipient { get; set; } = null!;

    public string Payload { get; set; } = null!;

    public string? ErrorMsg { get; set; }

    public DateTime? SentAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public NotifChannel Channel { get; set; }

    public NotifStatus Status { get; set; }

    public virtual User User { get; set; } = null!;
}
