using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewRentalCarManagerAPI.Common;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Infrastructure.MultiTenancy;
using NewRentalCarManagerAPI.Models;
using System.Net;

namespace NewRentalCarManagerAPI.Application.Features.Fleet;

public interface ICarBrandService
{
    Task<DataResult> GetAllAsync(FleetListInput input);
    Task<DataResult> GetByIdAsync(Guid id);
    Task<DataResult> CreateAsync(CreateCarBrandDto dto);
    Task<DataResult> UpdateAsync(Guid id, UpdateCarBrandDto dto);
    Task<DataResult> DeleteAsync(Guid id);
}

public class CarBrandService : ICarBrandService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<CarBrandService> _logger;
    private readonly ITenantProvider _tenantProvider;

    public CarBrandService(IUnitOfWork uow, IMapper mapper, ILogger<CarBrandService> logger, ITenantProvider tenantProvider)
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
            var query = _uow.CarBrands.Query().Where(x => x.TenantId == tenantId);
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(x => x.Name)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();
            return DataResult.ResultSuccess(_mapper.Map<List<CarBrandDto>>(items), "Get success!", totalCount);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get car brands");
            throw;
        }
    }

    public async Task<DataResult> GetByIdAsync(Guid id)
    {
        try
        {
            var tenantId = _tenantProvider.GetTenantIdOrThrow();
            var entity = await _uow.CarBrands.Query().FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "CarBrand not found!");
            return DataResult.ResultSuccess(_mapper.Map<CarBrandDto>(entity), "Get success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get car brand {Id}", id);
            throw;
        }
    }

    public async Task<DataResult> CreateAsync(CreateCarBrandDto dto)
    {
        try
        {
            var tenantId = _tenantProvider.GetTenantIdOrThrow();
            var entity = _mapper.Map<CarBrand>(dto);
            entity.TenantId = tenantId;
            await _uow.CarBrands.AddAsync(entity);
            return DataResult.ResultSuccess(_mapper.Map<CarBrandDto>(entity), "Insert success!", statusCode: 201);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create car brand");
            throw;
        }
    }

    public async Task<DataResult> UpdateAsync(Guid id, UpdateCarBrandDto dto)
    {
        try
        {
            var tenantId = _tenantProvider.GetTenantIdOrThrow();
            var entity = await _uow.CarBrands.Query().FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "CarBrand not found!");
            _mapper.Map(dto, entity);
            entity.TenantId = tenantId;
            return DataResult.ResultSuccess(_mapper.Map<CarBrandDto>(entity), "Update success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to update car brand {Id}", id);
            throw;
        }
    }

    public async Task<DataResult> DeleteAsync(Guid id)
    {
        try
        {
            var tenantId = _tenantProvider.GetTenantIdOrThrow();
            var entity = await _uow.CarBrands.Query().FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "CarBrand not found!");
            _uow.CarBrands.Remove(entity);
            return DataResult.ResultSuccess(true, "Delete success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to delete car brand {Id}", id);
            throw;
        }
    }
}