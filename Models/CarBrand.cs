using System;
using System.Collections.Generic;

namespace NewRentalCarManagerAPI.Models;

public partial class CarBrand
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? LogoUrl { get; set; }

    public virtual ICollection<CarModel> CarModels { get; set; } = new List<CarModel>();
}
