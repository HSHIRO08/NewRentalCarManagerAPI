using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewRentalCarManagerAPI.Common;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Enums;
using NewRentalCarManagerAPI.Infrastructure.MultiTenancy;
using NewRentalCarManagerAPI.Models;
using System.Net;

namespace NewRentalCarManagerAPI.Application.Features.Fleet;

public interface ICarService
{
    Task<DataResult> GetAllAsync(FleetListInput input);
    Task<DataResult> GetByIdAsync(Guid id);
    Task<DataResult> GetByOwnerAsync(Guid ownerId, FleetListInput input);
    Task<DataResult> CreateAsync(CreateCarDto dto);
    Task<DataResult> UpdateAsync(Guid id, UpdateCarDto dto);
    Task<DataResult> PatchStatusAsync(Guid id, string status);
    Task<DataResult> DeleteAsync(Guid id);
}

public class CarService : ICarService
{
    private readonly IRepository<Car> _carRepository;
    private readonly IRepository<CarModel> _carModelRepository;
    private readonly IRepository<Location> _locationRepository;
    private readonly IRepository<CarPricing> _carPricingRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CarService> _logger;
    private readonly ITenantProvider _tenantProvider;

    public CarService(IRepository<Car> carRepository, IRepository<CarModel> carModelRepository, IRepository<Location> locationRepository, IRepository<CarPricing> carPricingRepository, IMapper mapper, ILogger<CarService> logger, ITenantProvider tenantProvider)
    {
        _carRepository = carRepository;
        _carModelRepository = carModelRepository;
        _locationRepository = locationRepository;
        _carPricingRepository = carPricingRepository;
        _mapper = mapper;
        _logger = logger;
        _tenantProvider = tenantProvider;
    }

    private IQueryable<Car> FullQuery(int? tenantId = null)
    {
        var q = _carRepository.Query()
            .Include(c => c.Owner)
            .Include(c => c.Model).ThenInclude(m => m.Brand)
            .Include(c => c.Location)
            .Include(c => c.CarPricings);
        return tenantId.HasValue ? q.Where(c => c.TenantId == tenantId.Value) : q;
    }

    private IQueryable<Car> FreshQuery(int? tenantId = null)
    {
        var q = _carRepository.Query().AsNoTracking()
            .Include(c => c.Owner)
            .Include(c => c.Model).ThenInclude(m => m.Brand)
            .Include(c => c.Location)
            .Include(c => c.CarPricings);
        return tenantId.HasValue ? q.Where(c => c.TenantId == tenantId.Value) : q;
    }

    public async Task<DataResult> GetAllAsync(FleetListInput input)
    {
        try
        {
            var tenantId = _tenantProvider.TryGetTenantId();
            var query = FullQuery(tenantId);
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();
            return DataResult.ResultSuccess(_mapper.Map<List<CarDto>>(items), "Get success!", totalCount);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get cars");
            throw;
        }
    }

    public async Task<DataResult> GetByIdAsync(Guid id)
    {
        try
        {
            var tenantId = _tenantProvider.TryGetTenantId();
            var entity = await FullQuery(tenantId).FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Car not found!");
            return DataResult.ResultSuccess(_mapper.Map<CarDto>(entity), "Get success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get car {Id}", id);
            throw;
        }
    }

    public async Task<DataResult> GetByOwnerAsync(Guid ownerId, FleetListInput input)
    {
        try
        {
            var tenantId = _tenantProvider.TryGetTenantId();
            var query = FullQuery(tenantId).Where(c => c.OwnerId == ownerId);
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();
            return DataResult.ResultSuccess(_mapper.Map<List<CarDto>>(items), "Get success!", totalCount);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get cars by owner {OwnerId}", ownerId);
            throw;
        }
    }

    public async Task<DataResult> CreateAsync(CreateCarDto dto)
    {
        try
        {
            var tenantId = _tenantProvider.GetTenantIdOrThrow();
            var modelId = dto.ModelId ?? Guid.Empty;
            if (modelId == Guid.Empty && !string.IsNullOrWhiteSpace(dto.ModelName))
            {
                var model = await _carModelRepository.Query()
                    .Include(m => m.Brand)
                    .FirstOrDefaultAsync(m => m.Name == dto.ModelName
                        && m.TenantId == tenantId
                        && (dto.BrandName == null || m.Brand.Name == dto.BrandName));
                if (model is null)
                    throw new UserFriendlyException((int)HttpStatusCode.NotFound, $"Car model '{dto.ModelName}' not found.");
                modelId = model.Id;
            }
            else if (modelId != Guid.Empty)
            {
                var modelExists = await _carModelRepository.Query().AnyAsync(m => m.Id == modelId && m.TenantId == tenantId);
                if (!modelExists)
                {
                    throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Car model not found.");
                }
            }

            var locationId = dto.LocationId ?? Guid.Empty;
            if (locationId == Guid.Empty && !string.IsNullOrWhiteSpace(dto.Location))
            {
                var location = await _locationRepository.Query()
                    .FirstOrDefaultAsync(l => (l.City == dto.Location || l.Address == dto.Location) && l.TenantId == tenantId);
                if (location is null)
                {
                    location = new Location
                    {
                        Id = Guid.NewGuid(),
                        City = dto.Location,
                        IsActive = true,
                        TenantId = tenantId
                    };
                    await _locationRepository.AddAsync(location);
                }
                locationId = location.Id;
            }
            else if (locationId != Guid.Empty)
            {
                var locationExists = await _locationRepository.Query().AnyAsync(l => l.Id == locationId && l.TenantId == tenantId);
                if (!locationExists)
                {
                    throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Location not found.");
                }
            }

            var entity = new Car
            {
                TenantId = tenantId,
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
                FuelType = Enum.Parse<FuelType>(dto.FuelType, true),
                TransmissionType = Enum.Parse<TransmissionType>(dto.TransmissionType, true),
                Status = CarStatus.Available,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _carRepository.AddAsync(entity);

            if (dto.PricePerDay > 0)
            {
                var pricing = new CarPricing
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    CarId = entity.Id,
                    PriceVnd = dto.PricePerDay,
                    IsActive = true,
                    RentalType = RentalType.Daily
                };
                await _carPricingRepository.AddAsync(pricing);
            }

            var created = await FreshQuery(tenantId).FirstOrDefaultAsync(c => c.Id == entity.Id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.InternalServerError, "Create car failed.");
            return DataResult.ResultSuccess(_mapper.Map<CarDto>(created), "Insert success!", statusCode: 201);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create car");
            throw;
        }
    }

    public async Task<DataResult> UpdateAsync(Guid id, UpdateCarDto dto)
    {
        try
        {
            var tenantId = _tenantProvider.GetTenantIdOrThrow();
            var entity = await FullQuery(tenantId).FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Car not found!");

            var locationId = dto.LocationId ?? Guid.Empty;
            if (locationId == Guid.Empty && !string.IsNullOrWhiteSpace(dto.Location))
            {
                var location = await _locationRepository.Query()
                    .FirstOrDefaultAsync(l => (l.City == dto.Location || l.Address == dto.Location) && l.TenantId == tenantId);
                if (location is null)
                {
                    location = new Location { Id = Guid.NewGuid(), City = dto.Location, IsActive = true, TenantId = tenantId };
                    await _locationRepository.AddAsync(location);
                }
                locationId = location.Id;
            }
            else if (locationId != Guid.Empty)
            {
                var locationExists = await _locationRepository.Query().AnyAsync(l => l.Id == locationId && l.TenantId == tenantId);
                if (!locationExists)
                {
                    throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Location not found.");
                }
            }
            if (locationId != Guid.Empty)
            {
                entity.LocationId = locationId;
            }

            _mapper.Map(dto, entity);
            entity.TenantId = tenantId;

            if (!string.IsNullOrWhiteSpace(dto.Status) &&
                Enum.TryParse<CarStatus>(dto.Status, ignoreCase: true, out var parsedStatus))
            {
                entity.Status = parsedStatus;
            }

            if (!string.IsNullOrWhiteSpace(dto.FuelType) &&
                Enum.TryParse<FuelType>(dto.FuelType, ignoreCase: true, out var parsedFuel))
            {
                entity.FuelType = parsedFuel;
            }

            if (!string.IsNullOrWhiteSpace(dto.TransmissionType) &&
                Enum.TryParse<TransmissionType>(dto.TransmissionType, ignoreCase: true, out var parsedTx))
            {
                entity.TransmissionType = parsedTx;
            }

            var mergedImageUrls = dto.ImageUrls ?? new List<string>();
            if (!string.IsNullOrWhiteSpace(dto.ImageUrl) && !mergedImageUrls.Contains(dto.ImageUrl))
            {
                mergedImageUrls = new List<string>(mergedImageUrls) { dto.ImageUrl };
            }
            if (mergedImageUrls.Count > 0)
            {
                entity.ImageUrls = mergedImageUrls;
            }

            if (dto.PricePerDay.HasValue && dto.PricePerDay.Value > 0)
            {
                var pricing = await _carPricingRepository.Query()
                    .FirstOrDefaultAsync(p => p.CarId == id && p.IsActive && p.TenantId == tenantId);
                if (pricing is not null)
                {
                    pricing.PriceVnd = dto.PricePerDay.Value;
                }
                else
                {
                    var newPricing = new CarPricing
                    {
                        Id = Guid.NewGuid(),
                        TenantId = tenantId,
                        CarId = id,
                        PriceVnd = dto.PricePerDay.Value,
                        IsActive = true,
                        RentalType = RentalType.Daily
                    };
                    await _carPricingRepository.AddAsync(newPricing);
                }
            }

            entity.UpdatedAt = DateTime.UtcNow;

            var updated = await FreshQuery(tenantId).FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.InternalServerError, "Update car failed.");
            return DataResult.ResultSuccess(_mapper.Map<CarDto>(updated), "Update success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to update car {Id}", id);
            throw;
        }
    }

    public async Task<DataResult> PatchStatusAsync(Guid id, string status)
    {
        try
        {
            var tenantId = _tenantProvider.GetTenantIdOrThrow();
            var entity = await FullQuery(tenantId).FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Car not found!");

            if (!Enum.TryParse<CarStatus>(status, ignoreCase: true, out var parsedStatus))
            {
                throw new UserFriendlyException((int)HttpStatusCode.BadRequest, $"Invalid car status: {status}");
            }

            entity.Status = parsedStatus;
            entity.UpdatedAt = DateTime.UtcNow;
            return DataResult.ResultSuccess(_mapper.Map<CarDto>(entity), "Update status success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to patch car status {Id}", id);
            throw;
        }
    }

    public async Task<DataResult> DeleteAsync(Guid id)
    {
        try
        {
            var tenantId = _tenantProvider.GetTenantIdOrThrow();
            var entity = await _carRepository.Query().FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Car not found!");
            _carRepository.Remove(entity);
            return DataResult.ResultSuccess(true, "Delete success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to delete car {Id}", id);
            throw;
        }
    }
}