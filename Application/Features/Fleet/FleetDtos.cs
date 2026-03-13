namespace NewRentalCarManagerAPI.Application.Features.Fleet;

// ───── CarBrand ─────
public class CarBrandDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? LogoUrl { get; set; }
}
public class CreateCarBrandDto
{
    public string Name { get; set; } = null!;
    public string? LogoUrl { get; set; }
}
public class UpdateCarBrandDto
{
    public string Name { get; set; } = null!;
    public string? LogoUrl { get; set; }
}

// ───── CarModel ─────
public class CarModelDto
{
    public Guid Id { get; set; }
    public Guid BrandId { get; set; }
    public string BrandName { get; set; } = null!;
    public string Name { get; set; } = null!;
    public short SeatCount { get; set; }
    public string CarType { get; set; } = null!;
}
public class CreateCarModelDto
{
    public Guid BrandId { get; set; }
    public string Name { get; set; } = null!;
    public short SeatCount { get; set; }
    public string CarType { get; set; } = null!;
}
public class UpdateCarModelDto
{
    public string Name { get; set; } = null!;
    public short SeatCount { get; set; }
    public string CarType { get; set; } = null!;
}

// ───── Location ─────
public class LocationDto
{
    public Guid Id { get; set; }
    public string City { get; set; } = null!;
    public string? District { get; set; }
    public string? Ward { get; set; }
    public string? Address { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public bool IsActive { get; set; }
}
public class CreateLocationDto
{
    public string City { get; set; } = null!;
    public string? District { get; set; }
    public string? Ward { get; set; }
    public string? Address { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}
public class UpdateLocationDto
{
    public string City { get; set; } = null!;
    public string? District { get; set; }
    public string? Ward { get; set; }
    public string? Address { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public bool IsActive { get; set; }
}

// ───── Car ─────
public class CarDto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string OwnerName { get; set; } = null!;
    public Guid ModelId { get; set; }
    public string ModelName { get; set; } = null!;
    public string BrandName { get; set; } = null!;
    public Guid LocationId { get; set; }
    public string LicensePlate { get; set; } = null!;
    public short ManufactureYear { get; set; }
    public string? Color { get; set; }
    public int MileageKm { get; set; }
    public string? Description { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public List<string> Features { get; set; } = new();
    public bool HasIotDevice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
public class CreateCarDto
{
    public Guid OwnerId { get; set; }
    public Guid ModelId { get; set; }
    public Guid LocationId { get; set; }
    public string LicensePlate { get; set; } = null!;
    public short ManufactureYear { get; set; }
    public string? Color { get; set; }
    public int MileageKm { get; set; }
    public string? Description { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public List<string> Features { get; set; } = new();
    public bool HasIotDevice { get; set; }
    public string? IotDeviceId { get; set; }
}
public class UpdateCarDto
{
    public Guid LocationId { get; set; }
    public string? Color { get; set; }
    public int MileageKm { get; set; }
    public string? Description { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public List<string> Features { get; set; } = new();
    public bool HasIotDevice { get; set; }
    public string? IotDeviceId { get; set; }
}

// ───── CarPricing ─────
public class CarPricingDto
{
    public Guid Id { get; set; }
    public Guid CarId { get; set; }
    public short? DurationHours { get; set; }
    public int PriceVnd { get; set; }
    public bool IsActive { get; set; }
}
public class CreateCarPricingDto
{
    public Guid CarId { get; set; }
    public short? DurationHours { get; set; }
    public int PriceVnd { get; set; }
}
public class UpdateCarPricingDto
{
    public int PriceVnd { get; set; }
    public bool IsActive { get; set; }
}

// ───── CarAvailabilityBlock ─────
public class CarAvailabilityBlockDto
{
    public Guid Id { get; set; }
    public Guid CarId { get; set; }
    public DateTime BlockedFrom { get; set; }
    public DateTime BlockedTo { get; set; }
    public string Source { get; set; } = null!;
    public Guid? BookingId { get; set; }
    public string? Reason { get; set; }
}
public class CreateCarAvailabilityBlockDto
{
    public Guid CarId { get; set; }
    public DateTime BlockedFrom { get; set; }
    public DateTime BlockedTo { get; set; }
    public string Source { get; set; } = null!;
    public Guid? BookingId { get; set; }
    public string? Reason { get; set; }
}
