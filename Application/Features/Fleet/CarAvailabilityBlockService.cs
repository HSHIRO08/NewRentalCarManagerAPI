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
    private readonly IRepository<CarAvailabilityBlock> _carAvailabilityBlockRepository;
    private readonly IRepository<Car> _carRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CarAvailabilityBlockService> _logger;
    private readonly ITenantProvider _tenantProvider;

    public CarAvailabilityBlockService(IRepository<CarAvailabilityBlock> carAvailabilityBlockRepository, IRepository<Car> carRepository, IMapper mapper, ILogger<CarAvailabilityBlockService> logger, ITenantProvider tenantProvider)
    {
        _carAvailabilityBlockRepository = carAvailabilityBlockRepository;
        _carRepository = carRepository;
        _mapper = mapper;
        _logger = logger;
        _tenantProvider = tenantProvider;
    }

    public async Task<DataResult> GetByCarAsync(Guid carId, FleetListInput input)
    {
        try
        {
            var tenantId = _tenantProvider.TryGetTenantId();
            var query = _carAvailabilityBlockRepository.Query().Where(b => b.CarId == carId);
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
            var carExists = await _carRepository.Query().AnyAsync(x => x.Id == dto.CarId && x.TenantId == tenantId);
            if (!carExists)
            {
                throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Car not found!");
            }

            var entity = _mapper.Map<CarAvailabilityBlock>(dto);
            entity.TenantId = tenantId;
            await _carAvailabilityBlockRepository.AddAsync(entity);
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
            var entity = await _carAvailabilityBlockRepository.Query().FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "CarAvailabilityBlock not found!");
            _carAvailabilityBlockRepository.Remove(entity);
            return DataResult.ResultSuccess(true, "Delete success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to delete availability block {Id}", id);
            throw;
        }
    }
}