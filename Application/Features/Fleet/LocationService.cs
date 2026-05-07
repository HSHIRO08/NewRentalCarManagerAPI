using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewRentalCarManagerAPI.Common;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Infrastructure.MultiTenancy;
using NewRentalCarManagerAPI.Models;
using System.Net;

namespace NewRentalCarManagerAPI.Application.Features.Fleet;

public interface ILocationService
{
    Task<DataResult> GetAllAsync(FleetListInput input);
    Task<DataResult> GetByIdAsync(Guid id);
    Task<DataResult> CreateAsync(CreateLocationDto dto);
    Task<DataResult> UpdateAsync(Guid id, UpdateLocationDto dto);
    Task<DataResult> DeleteAsync(Guid id);
}

public class LocationService : ILocationService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<LocationService> _logger;
    private readonly ITenantProvider _tenantProvider;

    public LocationService(IUnitOfWork uow, IMapper mapper, ILogger<LocationService> logger, ITenantProvider tenantProvider)
    {
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
        _tenantProvider = tenantProvider;
    }

    public async Task<DataResult> GetAllAsync(FleetListInput input)
    {
        try
        {
            var tenantId = _tenantProvider.GetTenantIdOrThrow();
            var query = _uow.Locations.Query().Where(x => x.TenantId == tenantId);
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(x => x.City)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();
            return DataResult.ResultSuccess(_mapper.Map<List<LocationDto>>(items), "Get success!", totalCount);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get locations");
            throw;
        }
    }

    public async Task<DataResult> GetByIdAsync(Guid id)
    {
        try
        {
            var tenantId = _tenantProvider.GetTenantIdOrThrow();
            var entity = await _uow.Locations.Query().FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Location not found!");
            return DataResult.ResultSuccess(_mapper.Map<LocationDto>(entity), "Get success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get location {Id}", id);
            throw;
        }
    }

    public async Task<DataResult> CreateAsync(CreateLocationDto dto)
    {
        try
        {
            var tenantId = _tenantProvider.GetTenantIdOrThrow();
            var entity = _mapper.Map<Location>(dto);
            entity.IsActive = true;
            entity.TenantId = tenantId;
            await _uow.Locations.AddAsync(entity);
            return DataResult.ResultSuccess(_mapper.Map<LocationDto>(entity), "Insert success!", statusCode: 201);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create location");
            throw;
        }
    }

    public async Task<DataResult> UpdateAsync(Guid id, UpdateLocationDto dto)
    {
        try
        {
            var tenantId = _tenantProvider.GetTenantIdOrThrow();
            var entity = await _uow.Locations.Query().FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Location not found!");
            _mapper.Map(dto, entity);
            entity.TenantId = tenantId;
            return DataResult.ResultSuccess(_mapper.Map<LocationDto>(entity), "Update success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to update location {Id}", id);
            throw;
        }
    }

    public async Task<DataResult> DeleteAsync(Guid id)
    {
        try
        {
            var tenantId = _tenantProvider.GetTenantIdOrThrow();
            var entity = await _uow.Locations.Query().FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Location not found!");
            _uow.Locations.Remove(entity);
            return DataResult.ResultSuccess(true, "Delete success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to delete location {Id}", id);
            throw;
        }
    }
}