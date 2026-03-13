using System;
using System.Collections.Generic;

namespace NewRentalCarManagerAPI.Models;

public partial class Location
{
    public Guid Id { get; set; }

    public string City { get; set; } = null!;

    public string? District { get; set; }

    public string? Ward { get; set; }

    public string? Address { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Booking> BookingPickupLocations { get; set; } = new List<Booking>();

    public virtual ICollection<Booking> BookingReturnLocations { get; set; } = new List<Booking>();

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}
