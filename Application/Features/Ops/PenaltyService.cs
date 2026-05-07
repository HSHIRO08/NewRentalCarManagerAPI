using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewRentalCarManagerAPI.Common;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Models;
using System.Net;

namespace NewRentalCarManagerAPI.Application.Features.Ops;

public interface IPenaltyService
{
    Task<DataResult> GetByBookingAsync(Guid bookingId, OpsListInput input);
    Task<DataResult> GetByIdAsync(Guid id);
    Task<DataResult> CreateAsync(CreatePenaltyDto dto);
}

public class PenaltyService : IPenaltyService
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<PenaltyService> _logger;

    public PenaltyService(IUnitOfWork uow, ILogger<PenaltyService> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    private IQueryable<Penalty> BaseQuery() => _uow.Penalties.Query()
        .Include(p => p.ChargedToNavigation);

    public async Task<DataResult> GetByBookingAsync(Guid bookingId, OpsListInput input)
    {
        try
        {
            var query = BaseQuery().Where(p => p.BookingId == bookingId).OrderByDescending(p => p.CreatedAt);
            var totalCount = await query.CountAsync();
            var items = await query.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();
            return DataResult.ResultSuccess(items.Select(MapToDto).ToList(), "Get success!", totalCount);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get penalties by booking {BookingId}", bookingId);
            throw;
        }
    }

    public async Task<DataResult> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await BaseQuery().FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Penalty not found!");
            return DataResult.ResultSuccess(MapToDto(entity), "Get success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get penalty {Id}", id);
            throw;
        }
    }

    public async Task<DataResult> CreateAsync(CreatePenaltyDto dto)
    {
        try
        {
            var entity = new Penalty
            {
                BookingId = dto.BookingId,
                ChargedTo = dto.ChargedTo,
                AmountVnd = dto.AmountVnd,
                Description = dto.Description,
                EvidenceUrl = dto.EvidenceUrl,
                CreatedAt = DateTime.UtcNow
            };
            await _uow.Penalties.AddAsync(entity);
            var created = await BaseQuery().FirstOrDefaultAsync(p => p.Id == entity.Id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.InternalServerError, "Create penalty failed.");
            return DataResult.ResultSuccess(MapToDto(created), "Insert success!", statusCode: 201);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create penalty");
            throw;
        }
    }

    private static PenaltyDto MapToDto(Penalty e) => new()
    {
        Id = e.Id,
        BookingId = e.BookingId,
        ChargedTo = e.ChargedTo,
        ChargedToName = e.ChargedToNavigation.FullName,
        AmountVnd = e.AmountVnd,
        Description = e.Description,
        EvidenceUrl = e.EvidenceUrl,
        CreatedAt = e.CreatedAt
    };
}
