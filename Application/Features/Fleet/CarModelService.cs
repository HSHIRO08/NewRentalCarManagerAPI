using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewRentalCarManagerAPI.Common;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Infrastructure.MultiTenancy;
using NewRentalCarManagerAPI.Models;
using System.Net;

namespace NewRentalCarManagerAPI.Application.Features.Fleet;

public interface ICarModelService
{
    Task<DataResult> GetAllAsync(FleetListInput input);
    Task<DataResult> GetByBrandAsync(Guid brandId, FleetListInput input);
    Task<DataResult> GetByIdAsync(Guid id);
    Task<DataResult> CreateAsync(CreateCarModelDto dto);
    Task<DataResult> UpdateAsync(Guid id, UpdateCarModelDto dto);
    Task<DataResult> DeleteAsync(Guid id);
}

public class CarModelService : ICarModelService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<CarModelService> _logger;
    private readonly ITenantProvider _tenantProvider;

    public CarModelService(IUnitOfWork uow, IMapper mapper, ILogger<CarModelService> logger, ITenantProvider tenantProvider)
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
            var tenantId = _tenantProvider.TryGetTenantId();
            IQueryable<CarModel> query = _uow.CarModels.Query().Include(m => m.Brand);
            if (tenantId.HasValue) query = query.Where(m => m.TenantId == tenantId.Value);
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(x => x.Name)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();
            return DataResult.ResultSuccess(_mapper.Map<List<CarModelDto>>(items), "Get success!", totalCount);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get car models");
            throw;
        }
    }

    public async Task<DataResult> GetByBrandAsync(Guid brandId, FleetListInput input)
    {
        try
        {
            var tenantId = _tenantProvider.TryGetTenantId();
            IQueryable<CarModel> query = _uow.CarModels.Query().Include(m => m.Brand).Where(m => m.BrandId == brandId);
            if (tenantId.HasValue) query = query.Where(m => m.TenantId == tenantId.Value);
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(x => x.Name)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();
            return DataResult.ResultSuccess(_mapper.Map<List<CarModelDto>>(items), "Get success!", totalCount);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get car models by brand {BrandId}", brandId);
            throw;
        }
    }

    public async Task<DataResult> GetByIdAsync(Guid id)
    {
        try
        {
            var tenantId = _tenantProvider.TryGetTenantId();
            IQueryable<CarModel> query = _uow.CarModels.Query().Include(m => m.Brand).Where(m => m.Id == id);
            if (tenantId.HasValue) query = query.Where(m => m.TenantId == tenantId.Value);
            var entity = await query.FirstOrDefaultAsync()
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "CarModel not found!");
            return DataResult.ResultSuccess(_mapper.Map<CarModelDto>(entity), "Get success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get car model {Id}", id);
            throw;
        }
    }

    public async Task<DataResult> CreateAsync(CreateCarModelDto dto)
    {
        try
        {
            var tenantId = _tenantProvider.GetTenantIdOrThrow();
            var brandExists = await _uow.CarBrands.Query().AnyAsync(x => x.Id == dto.BrandId && x.TenantId == tenantId);
            if (!brandExists)
            {
                throw new UserFriendlyException((int)HttpStatusCode.NotFound, "CarBrand not found!");
            }

            var entity = _mapper.Map<CarModel>(dto);
            entity.TenantId = tenantId;
            await _uow.CarModels.AddAsync(entity);
            var created = await _uow.CarModels.Query()
                .Include(m => m.Brand)
                .FirstOrDefaultAsync(m => m.Id == entity.Id && m.TenantId == tenantId)
                ?? entity;
            return DataResult.ResultSuccess(_mapper.Map<CarModelDto>(created), "Insert success!", statusCode: 201);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create car model");
            throw;
        }
    }

    public async Task<DataResult> UpdateAsync(Guid id, UpdateCarModelDto dto)
    {
        try
        {
            var tenantId = _tenantProvider.GetTenantIdOrThrow();
            var entity = await _uow.CarModels.Query()
                .Include(m => m.Brand)
                .FirstOrDefaultAsync(m => m.Id == id && m.TenantId == tenantId)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "CarModel not found!");
            _mapper.Map(dto, entity);
            entity.TenantId = tenantId;
            return DataResult.ResultSuccess(_mapper.Map<CarModelDto>(entity), "Update success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to update car model {Id}", id);
            throw;
        }
    }

    public async Task<DataResult> DeleteAsync(Guid id)
    {
        try
        {
            var tenantId = _tenantProvider.GetTenantIdOrThrow();
            var entity = await _uow.CarModels.Query().FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "CarModel not found!");
            _uow.CarModels.Remove(entity);
            return DataResult.ResultSuccess(true, "Delete success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to delete car model {Id}", id);
            throw;
        }
    }
}