using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewRentalCarManagerAPI.Common;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Models;
using System.Net;

namespace NewRentalCarManagerAPI.Application.Features.Ops;

public interface IDamageReportService
{
    Task<DataResult> GetByBookingAsync(Guid bookingId, OpsListInput input);
    Task<DataResult> GetByIdAsync(Guid id);
    Task<DataResult> CreateAsync(CreateDamageReportDto dto);
    Task<DataResult> UpdateAsync(Guid id, UpdateDamageReportDto dto);
}

public class DamageReportService : IDamageReportService
{
    private readonly IRepository<DamageReport> _damageReportRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<DamageReportService> _logger;

    public DamageReportService(IRepository<DamageReport> damageReportRepository, IMapper mapper, ILogger<DamageReportService> logger)
    {
        _damageReportRepository = damageReportRepository;
        _mapper = mapper;
        _logger = logger;
    }

    private IQueryable<DamageReport> BaseQuery() => _damageReportRepository.Query()
        .Include(d => d.ReportedByNavigation);

    public async Task<DataResult> GetByBookingAsync(Guid bookingId, OpsListInput input)
    {
        try
        {
            var query = BaseQuery().Where(d => d.BookingId == bookingId).OrderByDescending(d => d.CreatedAt);
            var totalCount = await query.CountAsync();
            var items = await query.Skip(input.SkipCount).Take(input.MaxResultCount)
                .ProjectTo<DamageReportDto>(_mapper.ConfigurationProvider).ToListAsync();
            return DataResult.ResultSuccess(items, "Get success!", totalCount);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get damage reports by booking {BookingId}", bookingId);
            throw;
        }
    }

    public async Task<DataResult> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await BaseQuery().FirstOrDefaultAsync(d => d.Id == id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Damage report not found!");
            return DataResult.ResultSuccess(_mapper.Map<DamageReportDto>(entity), "Get success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get damage report {Id}", id);
            throw;
        }
    }

    public async Task<DataResult> CreateAsync(CreateDamageReportDto dto)
    {
        try
        {
            var entity = new DamageReport
            {
                BookingId = dto.BookingId,
                ReportedBy = dto.ReportedBy,
                Description = dto.Description,
                ImageUrls = dto.ImageUrls,
                RepairCostVnd = dto.RepairCostVnd,
                CreatedAt = DateTime.UtcNow
            };
            await _damageReportRepository.AddAsync(entity);
            var created = await BaseQuery().FirstOrDefaultAsync(d => d.Id == entity.Id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.InternalServerError, "Create damage report failed.");
            return DataResult.ResultSuccess(_mapper.Map<DamageReportDto>(created), "Insert success!", statusCode: 201);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create damage report");
            throw;
        }
    }

    public async Task<DataResult> UpdateAsync(Guid id, UpdateDamageReportDto dto)
    {
        try
        {
            var entity = await BaseQuery().FirstOrDefaultAsync(d => d.Id == id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Damage report not found!");
            entity.RepairCostVnd = dto.RepairCostVnd;
            entity.ResolvedAt = dto.ResolvedAt;
            return DataResult.ResultSuccess(_mapper.Map<DamageReportDto>(entity), "Update success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to update damage report {Id}", id);
            throw;
        }
    }

}
