using System;
using System.Collections.Generic;

namespace NewRentalCarManagerAPI.Models;

public partial class Permission
{
    public Guid Id { get; set; }

    public string Resource { get; set; } = null!;

    public string Action { get; set; } = null!;

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
