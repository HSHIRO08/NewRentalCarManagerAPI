using System;
using System.Collections.Generic;

namespace NewRentalCarManagerAPI.Models;

public partial class CarModel
{
    public Guid Id { get; set; }

    public Guid BrandId { get; set; }

    public string Name { get; set; } = null!;

    public short SeatCount { get; set; }

    public string CarType { get; set; } = null!;

    public virtual CarBrand Brand { get; set; } = null!;

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}
