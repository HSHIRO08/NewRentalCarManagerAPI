using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewRentalCarManagerAPI.Common;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Infrastructure.MultiTenancy;
using NewRentalCarManagerAPI.Models;
using System.Net;

namespace NewRentalCarManagerAPI.Application.Features.Fleet;

public interface ICarPricingService
{
    Task<DataResult> GetByCarAsync(Guid carId, FleetListInput input);
    Task<DataResult> GetByIdAsync(Guid id);
    Task<DataResult> CreateAsync(CreateCarPricingDto dto);
    Task<DataResult> UpdateAsync(Guid id, UpdateCarPricingDto dto);
    Task<DataResult> DeleteAsync(Guid id);
}

public class CarPricingService : ICarPricingService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<CarPricingService> _logger;
    private readonly ITenantProvider _tenantProvider;

    public CarPricingService(IUnitOfWork uow, IMapper mapper, ILogger<CarPricingService> logger, ITenantProvider tenantProvider)
    {
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
        _tenantProvider = tenantProvider;
    }

    public async Task<DataResult> GetByCarAsync(Guid carId, FleetListInput input)
    {
        try
        {
            var tenantId = _tenantProvider.TryGetTenantId();
            var query = _uow.CarPricings.Query().Where(p => p.CarId == carId);
            if (tenantId.HasValue) query = query.Where(p => p.TenantId == tenantId.Value);
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(p => p.IsActive)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();
            return DataResult.ResultSuccess(_mapper.Map<List<CarPricingDto>>(items), "Get success!", totalCount);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get car pricings by car {CarId}", carId);
            throw;
        }
    }

    public async Task<DataResult> GetByIdAsync(Guid id)
    {
        try
        {
            var tenantId = _tenantProvider.TryGetTenantId();
            var query = _uow.CarPricings.Query().Where(x => x.Id == id);
            if (tenantId.HasValue) query = query.Where(x => x.TenantId == tenantId.Value);
            var entity = await query.FirstOrDefaultAsync()
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "CarPricing not found!");
            return DataResult.ResultSuccess(_mapper.Map<CarPricingDto>(entity), "Get success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get car pricing {Id}", id);
            throw;
        }
    }

    public async Task<DataResult> CreateAsync(CreateCarPricingDto dto)
    {
        try
        {
            var tenantId = _tenantProvider.GetTenantIdOrThrow();
            var carExists = await _uow.Cars.Query().AnyAsync(x => x.Id == dto.CarId && x.TenantId == tenantId);
            if (!carExists)
            {
                throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Car not found!");
            }

            var entity = _mapper.Map<CarPricing>(dto);
            entity.IsActive = true;
            entity.TenantId = tenantId;
            await _uow.CarPricings.AddAsync(entity);
            return DataResult.ResultSuccess(_mapper.Map<CarPricingDto>(entity), "Insert success!", statusCode: 201);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create car pricing");
            throw;
        }
    }

    public async Task<DataResult> UpdateAsync(Guid id, UpdateCarPricingDto dto)
    {
        try
        {
            var tenantId = _tenantProvider.GetTenantIdOrThrow();
            var entity = await _uow.CarPricings.Query().FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "CarPricing not found!");
            _mapper.Map(dto, entity);
            entity.TenantId = tenantId;
            return DataResult.ResultSuccess(_mapper.Map<CarPricingDto>(entity), "Update success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to update car pricing {Id}", id);
            throw;
        }
    }

    public async Task<DataResult> DeleteAsync(Guid id)
    {
        try
        {
            var tenantId = _tenantProvider.GetTenantIdOrThrow();
            var entity = await _uow.CarPricings.Query().FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "CarPricing not found!");
            _uow.CarPricings.Remove(entity);
            return DataResult.ResultSuccess(true, "Delete success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to delete car pricing {Id}", id);
            throw;
        }
    }
}