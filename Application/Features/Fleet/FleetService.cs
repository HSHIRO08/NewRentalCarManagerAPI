using Microsoft.EntityFrameworkCore;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Models;

namespace NewRentalCarManagerAPI.Application.Features.Fleet;

// ══════════════════════════════════════════════
//  CarBrand
// ══════════════════════════════════════════════
public interface ICarBrandService
{
    Task<IEnumerable<CarBrandDto>> GetAllAsync();
    Task<CarBrandDto?> GetByIdAsync(Guid id);
    Task<CarBrandDto> CreateAsync(CreateCarBrandDto dto);
    Task<CarBrandDto?> UpdateAsync(Guid id, UpdateCarBrandDto dto);
    Task<bool> DeleteAsync(Guid id);
}

public class CarBrandService : ICarBrandService
{
    private readonly IUnitOfWork _uow;
    public CarBrandService(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<CarBrandDto>> GetAllAsync()
    {
        var items = await _uow.CarBrands.GetAllAsync();
        return items.Select(MapToDto);
    }

    public async Task<CarBrandDto?> GetByIdAsync(Guid id)
    {
        var e = await _uow.CarBrands.GetByIdAsync(id);
        return e is null ? null : MapToDto(e);
    }

    public async Task<CarBrandDto> CreateAsync(CreateCarBrandDto dto)
    {
        var e = new CarBrand { Name = dto.Name, LogoUrl = dto.LogoUrl };
        await _uow.CarBrands.AddAsync(e);
        return MapToDto(e);
    }

    public async Task<CarBrandDto?> UpdateAsync(Guid id, UpdateCarBrandDto dto)
    {
        var e = await _uow.CarBrands.GetByIdAsync(id);
        if (e is null) return null;
        e.Name = dto.Name;
        e.LogoUrl = dto.LogoUrl;
        return MapToDto(e);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var e = await _uow.CarBrands.GetByIdAsync(id);
        if (e is null) return false;
        _uow.CarBrands.Remove(e);
        return true;
    }

    private static CarBrandDto MapToDto(CarBrand e) => new()
    { Id = e.Id, Name = e.Name, LogoUrl = e.LogoUrl };
}

// ══════════════════════════════════════════════
//  CarModel
// ══════════════════════════════════════════════
public interface ICarModelService
{
    Task<IEnumerable<CarModelDto>> GetAllAsync();
    Task<IEnumerable<CarModelDto>> GetByBrandAsync(Guid brandId);
    Task<CarModelDto?> GetByIdAsync(Guid id);
    Task<CarModelDto> CreateAsync(CreateCarModelDto dto);
    Task<CarModelDto?> UpdateAsync(Guid id, UpdateCarModelDto dto);
    Task<bool> DeleteAsync(Guid id);
}

public class CarModelService : ICarModelService
{
    private readonly IUnitOfWork _uow;
    public CarModelService(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<CarModelDto>> GetAllAsync()
    {
        var items = await _uow.CarModels.Query().Include(m => m.Brand).ToListAsync();
        return items.Select(MapToDto);
    }

    public async Task<IEnumerable<CarModelDto>> GetByBrandAsync(Guid brandId)
    {
        var items = await _uow.CarModels.Query()
            .Include(m => m.Brand).Where(m => m.BrandId == brandId).ToListAsync();
        return items.Select(MapToDto);
    }

    public async Task<CarModelDto?> GetByIdAsync(Guid id)
    {
        var e = await _uow.CarModels.Query().Include(m => m.Brand).FirstOrDefaultAsync(m => m.Id == id);
        return e is null ? null : MapToDto(e);
    }

    public async Task<CarModelDto> CreateAsync(CreateCarModelDto dto)
    {
        var e = new CarModel
        {
            BrandId = dto.BrandId, Name = dto.Name,
            SeatCount = dto.SeatCount, CarType = dto.CarType
        };
        await _uow.CarModels.AddAsync(e);
        e.Brand = (await _uow.CarBrands.GetByIdAsync(dto.BrandId))!;
        return MapToDto(e);
    }

    public async Task<CarModelDto?> UpdateAsync(Guid id, UpdateCarModelDto dto)
    {
        var e = await _uow.CarModels.Query().Include(m => m.Brand).FirstOrDefaultAsync(m => m.Id == id);
        if (e is null) return null;
        e.Name = dto.Name;
        e.SeatCount = dto.SeatCount;
        e.CarType = dto.CarType;
        return MapToDto(e);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var e = await _uow.CarModels.GetByIdAsync(id);
        if (e is null) return false;
        _uow.CarModels.Remove(e);
        return true;
    }

    private static CarModelDto MapToDto(CarModel e) => new()
    {
        Id = e.Id, BrandId = e.BrandId, BrandName = e.Brand.Name,
        Name = e.Name, SeatCount = e.SeatCount, CarType = e.CarType
    };
}

// ══════════════════════════════════════════════
//  Location
// ══════════════════════════════════════════════
public interface ILocationService
{
    Task<IEnumerable<LocationDto>> GetAllAsync();
    Task<LocationDto?> GetByIdAsync(Guid id);
    Task<LocationDto> CreateAsync(CreateLocationDto dto);
    Task<LocationDto?> UpdateAsync(Guid id, UpdateLocationDto dto);
    Task<bool> DeleteAsync(Guid id);
}

public class LocationService : ILocationService
{
    private readonly IUnitOfWork _uow;
    public LocationService(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<LocationDto>> GetAllAsync()
    {
        var items = await _uow.Locations.GetAllAsync();
        return items.Select(MapToDto);
    }

    public async Task<LocationDto?> GetByIdAsync(Guid id)
    {
        var e = await _uow.Locations.GetByIdAsync(id);
        return e is null ? null : MapToDto(e);
    }

    public async Task<LocationDto> CreateAsync(CreateLocationDto dto)
    {
        var e = new Location
        {
            City = dto.City, District = dto.District, Ward = dto.Ward,
            Address = dto.Address, Latitude = dto.Latitude,
            Longitude = dto.Longitude, IsActive = true
        };
        await _uow.Locations.AddAsync(e);
        return MapToDto(e);
    }

    public async Task<LocationDto?> UpdateAsync(Guid id, UpdateLocationDto dto)
    {
        var e = await _uow.Locations.GetByIdAsync(id);
        if (e is null) return null;
        e.City = dto.City; e.District = dto.District; e.Ward = dto.Ward;
        e.Address = dto.Address; e.Latitude = dto.Latitude;
        e.Longitude = dto.Longitude; e.IsActive = dto.IsActive;
        return MapToDto(e);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var e = await _uow.Locations.GetByIdAsync(id);
        if (e is null) return false;
        _uow.Locations.Remove(e);
        return true;
    }

    private static LocationDto MapToDto(Location e) => new()
    {
        Id = e.Id, City = e.City, District = e.District, Ward = e.Ward,
        Address = e.Address, Latitude = e.Latitude, Longitude = e.Longitude, IsActive = e.IsActive
    };
}

// ══════════════════════════════════════════════
//  Car
// ══════════════════════════════════════════════
public interface ICarService
{
    Task<IEnumerable<CarDto>> GetAllAsync();
    Task<CarDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<CarDto>> GetByOwnerAsync(Guid ownerId);
    Task<CarDto> CreateAsync(CreateCarDto dto);
    Task<CarDto?> UpdateAsync(Guid id, UpdateCarDto dto);
    Task<CarDto?> PatchStatusAsync(Guid id, string status);
    Task<bool> DeleteAsync(Guid id);
}

public class CarService : ICarService
{
    private readonly IUnitOfWork _uow;
    public CarService(IUnitOfWork uow) => _uow = uow;

    private IQueryable<Car> FullQuery() => _uow.Cars.Query()
        .Include(c => c.Owner)
        .Include(c => c.Model).ThenInclude(m => m.Brand)
        .Include(c => c.Location)
        .Include(c => c.CarPricings);

    private IQueryable<Car> FreshQuery() => _uow.Cars.Query().AsNoTracking()
        .Include(c => c.Owner)
        .Include(c => c.Model).ThenInclude(m => m.Brand)
        .Include(c => c.Location)
        .Include(c => c.CarPricings);

    public async Task<IEnumerable<CarDto>> GetAllAsync()
    {
        var items = await FullQuery().ToListAsync();
        return items.Select(MapToDto);
    }

    public async Task<CarDto?> GetByIdAsync(Guid id)
    {
        var e = await FullQuery().FirstOrDefaultAsync(c => c.Id == id);
        return e is null ? null : MapToDto(e);
    }

    public async Task<IEnumerable<CarDto>> GetByOwnerAsync(Guid ownerId)
    {
        var items = await FullQuery().Where(c => c.OwnerId == ownerId).ToListAsync();
        return items.Select(MapToDto);
    }

    public async Task<CarDto> CreateAsync(CreateCarDto dto)
    {
        // Resolve ModelId from brand/model names if not provided
        var modelId = dto.ModelId ?? Guid.Empty;
        if (modelId == Guid.Empty && !string.IsNullOrWhiteSpace(dto.ModelName))
        {
            var model = await _uow.CarModels.Query()
                .Include(m => m.Brand)
                .FirstOrDefaultAsync(m => m.Name == dto.ModelName
                    && (dto.BrandName == null || m.Brand.Name == dto.BrandName));
            if (model is null)
                throw new ArgumentException($"Car model '{dto.ModelName}' not found.");
            modelId = model.Id;
        }

        // Resolve LocationId from location string if not provided
        var locationId = dto.LocationId ?? Guid.Empty;
        if (locationId == Guid.Empty && !string.IsNullOrWhiteSpace(dto.Location))
        {
            var location = await _uow.Locations.Query()
                .FirstOrDefaultAsync(l => l.City == dto.Location || l.Address == dto.Location);
            if (location is null)
            {
                // Auto-create location
                location = new Location
                {
                    Id = Guid.NewGuid(),
                    City = dto.Location,
                    IsActive = true
                };
                await _uow.Locations.AddAsync(location);
                await _uow.SaveChangesAsync();
            }
            locationId = location.Id;
        }

        var e = new Car
        {
            OwnerId = dto.OwnerId,
            ModelId = modelId,
            LocationId = locationId,
            LicensePlate = dto.LicensePlate,
            ManufactureYear = dto.ManufactureYear,
            Color = dto.Color,
            MileageKm = dto.MileageKm,
            Description = dto.Description,
            ImageUrls = (!string.IsNullOrWhiteSpace(dto.ImageUrl) && !dto.ImageUrls.Contains(dto.ImageUrl))
                ? new List<string>(dto.ImageUrls) { dto.ImageUrl }
                : dto.ImageUrls,
            Features = dto.Features,
            HasIotDevice = dto.HasIotDevice,
            IotDeviceId = dto.IotDeviceId,
            FuelType = Enum.Parse<Enums.FuelType>(dto.FuelType, true),
            TransmissionType = Enum.Parse<Enums.TransmissionType>(dto.TransmissionType, true),
            Status = Enums.CarStatus.Available,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _uow.Cars.AddAsync(e);
        await _uow.SaveChangesAsync();

        // Create initial pricing
        if (dto.PricePerDay > 0)
        {
            var pricing = new CarPricing
            {
                Id = Guid.NewGuid(),
                CarId = e.Id,
                PriceVnd = dto.PricePerDay,
                IsActive = true,
                RentalType = Enums.RentalType.Daily
            };
            await _uow.CarPricings.AddAsync(pricing);
            await _uow.SaveChangesAsync();
        }

        var created = await FreshQuery().FirstOrDefaultAsync(c => c.Id == e.Id);
        return MapToDto(created!);
    }

    public async Task<CarDto?> UpdateAsync(Guid id, UpdateCarDto dto)
    {
        var e = await FullQuery().FirstOrDefaultAsync(c => c.Id == id);
        if (e is null) return null;

        // Resolve LocationId from string if provided
        var locationId = dto.LocationId ?? Guid.Empty;
        if (locationId == Guid.Empty && !string.IsNullOrWhiteSpace(dto.Location))
        {
            var location = await _uow.Locations.Query()
                .FirstOrDefaultAsync(l => l.City == dto.Location || l.Address == dto.Location);
            if (location is null)
            {
                location = new Location { Id = Guid.NewGuid(), City = dto.Location, IsActive = true };
                await _uow.Locations.AddAsync(location);
                await _uow.SaveChangesAsync();
            }
            locationId = location.Id;
        }
        if (locationId != Guid.Empty) e.LocationId = locationId;

        // Update status
        if (!string.IsNullOrWhiteSpace(dto.Status) &&
            Enum.TryParse<Enums.CarStatus>(dto.Status, ignoreCase: true, out var parsedStatus))
            e.Status = parsedStatus;

        // Update other fields
        if (dto.Color is not null) e.Color = dto.Color;
        e.MileageKm = dto.MileageKm;
        if (dto.Description is not null) e.Description = dto.Description;

        // Merge imageUrl into imageUrls
        var imageUrls = dto.ImageUrls ?? new List<string>();
        if (!string.IsNullOrWhiteSpace(dto.ImageUrl) && !imageUrls.Contains(dto.ImageUrl))
            imageUrls = new List<string>(imageUrls) { dto.ImageUrl };
        if (imageUrls.Count > 0) e.ImageUrls = imageUrls;

        if (dto.Features.Count > 0) e.Features = dto.Features;
        if (dto.LicensePlate is not null) e.LicensePlate = dto.LicensePlate;
        if (dto.ManufactureYear.HasValue) e.ManufactureYear = dto.ManufactureYear.Value;
        if (!string.IsNullOrWhiteSpace(dto.FuelType) &&
            Enum.TryParse<Enums.FuelType>(dto.FuelType, ignoreCase: true, out var parsedFuel))
            e.FuelType = parsedFuel;
        if (!string.IsNullOrWhiteSpace(dto.TransmissionType) &&
            Enum.TryParse<Enums.TransmissionType>(dto.TransmissionType, ignoreCase: true, out var parsedTx))
            e.TransmissionType = parsedTx;
        e.HasIotDevice = dto.HasIotDevice;
        if (dto.IotDeviceId is not null) e.IotDeviceId = dto.IotDeviceId;

        // Update price: upsert the active CarPricing record directly from DB
        if (dto.PricePerDay.HasValue && dto.PricePerDay.Value > 0)
        {
            var pricing = await _uow.CarPricings.Query()
                .FirstOrDefaultAsync(p => p.CarId == id && p.IsActive);
            if (pricing is not null)
            {
                pricing.PriceVnd = dto.PricePerDay.Value;
            }
            else
            {
                var newPricing = new CarPricing
                {
                    Id = Guid.NewGuid(),
                    CarId = id,
                    PriceVnd = dto.PricePerDay.Value,
                    IsActive = true,
                    RentalType = Enums.RentalType.Daily
                };
                await _uow.CarPricings.AddAsync(newPricing);
            }
        }

        e.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync();
        
        var updated = await FreshQuery().FirstOrDefaultAsync(c => c.Id == id);
        return updated is null ? null : MapToDto(updated);
    }

    public async Task<CarDto?> PatchStatusAsync(Guid id, string status)
    {
        var e = await FullQuery().FirstOrDefaultAsync(c => c.Id == id);
        if (e is null) return null;
        if (!Enum.TryParse<Enums.CarStatus>(status, ignoreCase: true, out var parsed))
            throw new ArgumentException($"Invalid car status: {status}");
        e.Status = parsed;
        e.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync();
        return MapToDto(e);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var e = await _uow.Cars.GetByIdAsync(id);
        if (e is null) return false;
        _uow.Cars.Remove(e);
        return true;
    }

    private static CarDto MapToDto(Car e) => new()
    {
        Id = e.Id, OwnerId = e.OwnerId, OwnerName = e.Owner?.FullName ?? "",
        ModelId = e.ModelId, ModelName = e.Model?.Name ?? "", BrandName = e.Model?.Brand?.Name ?? "",
        LocationId = e.LocationId, LicensePlate = e.LicensePlate,
        ManufactureYear = e.ManufactureYear, Color = e.Color,
        MileageKm = e.MileageKm, Description = e.Description,
        ImageUrls = e.ImageUrls, Features = e.Features,
        HasIotDevice = e.HasIotDevice,
        CreatedAt = e.CreatedAt, UpdatedAt = e.UpdatedAt,
        PricePerDay = e.CarPricings?.FirstOrDefault(p => p.IsActive)?.PriceVnd ?? 1000000,
        Seats = e.Model?.SeatCount ?? 4,
        Transmission = e.TransmissionType.ToString(),
        Status = e.Status.ToString(),
        Location = e.Location?.City ?? ""
    };
}

// ══════════════════════════════════════════════
//  CarPricing
// ══════════════════════════════════════════════
public interface ICarPricingService
{
    Task<IEnumerable<CarPricingDto>> GetByCarAsync(Guid carId);
    Task<CarPricingDto?> GetByIdAsync(Guid id);
    Task<CarPricingDto> CreateAsync(CreateCarPricingDto dto);
    Task<CarPricingDto?> UpdateAsync(Guid id, UpdateCarPricingDto dto);
    Task<bool> DeleteAsync(Guid id);
}

public class CarPricingService : ICarPricingService
{
    private readonly IUnitOfWork _uow;
    public CarPricingService(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<CarPricingDto>> GetByCarAsync(Guid carId)
    {
        var items = await _uow.CarPricings.Query().Where(p => p.CarId == carId).ToListAsync();
        return items.Select(MapToDto);
    }

    public async Task<CarPricingDto?> GetByIdAsync(Guid id)
    {
        var e = await _uow.CarPricings.GetByIdAsync(id);
        return e is null ? null : MapToDto(e);
    }

    public async Task<CarPricingDto> CreateAsync(CreateCarPricingDto dto)
    {
        var e = new CarPricing { CarId = dto.CarId, DurationHours = dto.DurationHours, PriceVnd = dto.PriceVnd, IsActive = true };
        await _uow.CarPricings.AddAsync(e);
        return MapToDto(e);
    }

    public async Task<CarPricingDto?> UpdateAsync(Guid id, UpdateCarPricingDto dto)
    {
        var e = await _uow.CarPricings.GetByIdAsync(id);
        if (e is null) return null;
        e.PriceVnd = dto.PriceVnd; e.IsActive = dto.IsActive;
        return MapToDto(e);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var e = await _uow.CarPricings.GetByIdAsync(id);
        if (e is null) return false;
        _uow.CarPricings.Remove(e);
        return true;
    }

    private static CarPricingDto MapToDto(CarPricing e) => new()
    { Id = e.Id, CarId = e.CarId, DurationHours = e.DurationHours, PriceVnd = e.PriceVnd, IsActive = e.IsActive };
}

// ══════════════════════════════════════════════
//  CarAvailabilityBlock
// ══════════════════════════════════════════════
public interface ICarAvailabilityBlockService
{
    Task<IEnumerable<CarAvailabilityBlockDto>> GetByCarAsync(Guid carId);
    Task<CarAvailabilityBlockDto> CreateAsync(CreateCarAvailabilityBlockDto dto);
    Task<bool> DeleteAsync(Guid id);
}

public class CarAvailabilityBlockService : ICarAvailabilityBlockService
{
    private readonly IUnitOfWork _uow;
    public CarAvailabilityBlockService(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<CarAvailabilityBlockDto>> GetByCarAsync(Guid carId)
    {
        var items = await _uow.CarAvailabilityBlocks.Query()
            .Where(b => b.CarId == carId).OrderBy(b => b.BlockedFrom).ToListAsync();
        return items.Select(MapToDto);
    }

    public async Task<CarAvailabilityBlockDto> CreateAsync(CreateCarAvailabilityBlockDto dto)
    {
        var e = new CarAvailabilityBlock
        {
            CarId = dto.CarId, BlockedFrom = dto.BlockedFrom, BlockedTo = dto.BlockedTo,
            Source = dto.Source, BookingId = dto.BookingId, Reason = dto.Reason
        };
        await _uow.CarAvailabilityBlocks.AddAsync(e);
        return MapToDto(e);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var e = await _uow.CarAvailabilityBlocks.GetByIdAsync(id);
        if (e is null) return false;
        _uow.CarAvailabilityBlocks.Remove(e);
        return true;
    }

    private static CarAvailabilityBlockDto MapToDto(CarAvailabilityBlock e) => new()
    {
        Id = e.Id, CarId = e.CarId, BlockedFrom = e.BlockedFrom,
        BlockedTo = e.BlockedTo, Source = e.Source, BookingId = e.BookingId, Reason = e.Reason
    };
}
