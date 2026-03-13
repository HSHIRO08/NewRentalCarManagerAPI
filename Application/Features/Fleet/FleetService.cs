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
    Task<bool> DeleteAsync(Guid id);
}

public class CarService : ICarService
{
    private readonly IUnitOfWork _uow;
    public CarService(IUnitOfWork uow) => _uow = uow;

    private IQueryable<Car> FullQuery() => _uow.Cars.Query()
        .Include(c => c.Owner)
        .Include(c => c.Model).ThenInclude(m => m.Brand)
        .Include(c => c.Location);

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
        var e = new Car
        {
            OwnerId = dto.OwnerId, ModelId = dto.ModelId, LocationId = dto.LocationId,
            LicensePlate = dto.LicensePlate, ManufactureYear = dto.ManufactureYear,
            Color = dto.Color, MileageKm = dto.MileageKm, Description = dto.Description,
            ImageUrls = dto.ImageUrls, Features = dto.Features,
            HasIotDevice = dto.HasIotDevice, IotDeviceId = dto.IotDeviceId,
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        };
        await _uow.Cars.AddAsync(e);
        return (await GetByIdAsync(e.Id))!;
    }

    public async Task<CarDto?> UpdateAsync(Guid id, UpdateCarDto dto)
    {
        var e = await FullQuery().FirstOrDefaultAsync(c => c.Id == id);
        if (e is null) return null;
        e.LocationId = dto.LocationId; e.Color = dto.Color;
        e.MileageKm = dto.MileageKm; e.Description = dto.Description;
        e.ImageUrls = dto.ImageUrls; e.Features = dto.Features;
        e.HasIotDevice = dto.HasIotDevice; e.IotDeviceId = dto.IotDeviceId;
        e.UpdatedAt = DateTime.UtcNow;
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
        Id = e.Id, OwnerId = e.OwnerId, OwnerName = e.Owner.FullName,
        ModelId = e.ModelId, ModelName = e.Model.Name, BrandName = e.Model.Brand.Name,
        LocationId = e.LocationId, LicensePlate = e.LicensePlate,
        ManufactureYear = e.ManufactureYear, Color = e.Color,
        MileageKm = e.MileageKm, Description = e.Description,
        ImageUrls = e.ImageUrls, Features = e.Features,
        HasIotDevice = e.HasIotDevice,
        CreatedAt = e.CreatedAt, UpdatedAt = e.UpdatedAt
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
