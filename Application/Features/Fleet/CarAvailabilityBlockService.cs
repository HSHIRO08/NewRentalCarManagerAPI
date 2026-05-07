using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewRentalCarManagerAPI.Common;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Infrastructure.MultiTenancy;
using NewRentalCarManagerAPI.Models;
using System.Net;

namespace NewRentalCarManagerAPI.Application.Features.Fleet;

public interface ICarAvailabilityBlockService
{
    Task<DataResult> GetByCarAsync(Guid carId, FleetListInput input);
    Task<DataResult> CreateAsync(CreateCarAvailabilityBlockDto dto);
    Task<DataResult> DeleteAsync(Guid id);
}

public class CarAvailabilityBlockService : ICarAvailabilityBlockService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<CarAvailabilityBlockService> _logger;
    private readonly ITenantProvider _tenantProvider;

    public CarAvailabilityBlockService(IUnitOfWork uow, IMapper mapper, ILogger<CarAvailabilityBlockService> logger, ITenantProvider tenantProvider)
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
            var query = _uow.CarAvailabilityBlocks.Query().Where(b => b.CarId == carId);
            if (tenantId.HasValue) query = query.Where(b => b.TenantId == tenantId.Value);
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(b => b.BlockedFrom)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();
            return DataResult.ResultSuccess(_mapper.Map<List<CarAvailabilityBlockDto>>(items), "Get success!", totalCount);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get availability blocks by car {CarId}", carId);
            throw;
        }
    }

    public async Task<DataResult> CreateAsync(CreateCarAvailabilityBlockDto dto)
    {
        try
        {
            var tenantId = _tenantProvider.GetTenantIdOrThrow();
            var carExists = await _uow.Cars.Query().AnyAsync(x => x.Id == dto.CarId && x.TenantId == tenantId);
            if (!carExists)
            {
                throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Car not found!");
            }

            var entity = _mapper.Map<CarAvailabilityBlock>(dto);
            entity.TenantId = tenantId;
            await _uow.CarAvailabilityBlocks.AddAsync(entity);
            return DataResult.ResultSuccess(_mapper.Map<CarAvailabilityBlockDto>(entity), "Insert success!", statusCode: 201);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create availability block");
            throw;
        }
    }

    public async Task<DataResult> DeleteAsync(Guid id)
    {
        try
        {
            var tenantId = _tenantProvider.GetTenantIdOrThrow();
            var entity = await _uow.CarAvailabilityBlocks.Query().FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "CarAvailabilityBlock not found!");
            _uow.CarAvailabilityBlocks.Remove(entity);
            return DataResult.ResultSuccess(true, "Delete success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to delete availability block {Id}", id);
            throw;
        }
    }
}