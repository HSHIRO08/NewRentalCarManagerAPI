using System;
using System.Collections.Generic;
using NewRentalCarManagerAPI.Enums;

namespace NewRentalCarManagerAPI.Models;

public partial class Car
{
    public Guid Id { get; set; }

    public Guid OwnerId { get; set; }

    public Guid ModelId { get; set; }

    public Guid LocationId { get; set; }

    public string LicensePlate { get; set; } = null!;

    public short ManufactureYear { get; set; }

    public string? Color { get; set; }

    public int MileageKm { get; set; }

    public string? Description { get; set; }

    public List<string> ImageUrls { get; set; } = null!;

    public List<string> Features { get; set; } = null!;

    public bool HasIotDevice { get; set; }

    public string? IotDeviceId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public CarStatus Status { get; set; }

    public FuelType FuelType { get; set; }

    public TransmissionType TransmissionType { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<CarAvailabilityBlock> CarAvailabilityBlocks { get; set; } = new List<CarAvailabilityBlock>();

    public virtual ICollection<CarPricing> CarPricings { get; set; } = new List<CarPricing>();

    public virtual Location Location { get; set; } = null!;

    public virtual CarModel Model { get; set; } = null!;

    public virtual User Owner { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
