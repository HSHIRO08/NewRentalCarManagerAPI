using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewRentalCarManagerAPI.Common;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Models;
using System.Net;

namespace NewRentalCarManagerAPI.Application.Features.Payments;

public class OwnerPayoutService : IOwnerPayoutService
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<OwnerPayoutService> _logger;

    public OwnerPayoutService(IUnitOfWork uow, ILogger<OwnerPayoutService> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    private IQueryable<OwnerPayout> BaseQuery() => _uow.OwnerPayouts.Query()
        .Include(p => p.Owner);

    public async Task<DataResult> GetByOwnerAsync(Guid ownerId, PaymentListInput input)
    {
        try
        {
            var query = BaseQuery().Where(p => p.OwnerId == ownerId).OrderByDescending(p => p.PeriodTo);
            var totalCount = await query.CountAsync();
            var items = await query.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();
            return DataResult.ResultSuccess(items.Select(MapToDto).ToList(), "Get success!", totalCount);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get owner payouts by owner {OwnerId}", ownerId);
            throw;
        }
    }

    public async Task<DataResult> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await BaseQuery().FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Owner payout not found!");
            return DataResult.ResultSuccess(MapToDto(entity), "Get success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get owner payout {Id}", id);
            throw;
        }
    }

    public async Task<DataResult> CreateAsync(CreateOwnerPayoutDto dto)
    {
        try
        {
            var entity = new OwnerPayout
            {
                OwnerId = dto.OwnerId,
                PeriodFrom = dto.PeriodFrom,
                PeriodTo = dto.PeriodTo,
                GrossAmountVnd = dto.GrossAmountVnd,
                PlatformFeeVnd = dto.PlatformFeeVnd,
                NetAmountVnd = dto.GrossAmountVnd - dto.PlatformFeeVnd,
                BankAccount = dto.BankAccount
            };
            await _uow.OwnerPayouts.AddAsync(entity);
            var created = await BaseQuery().FirstOrDefaultAsync(p => p.Id == entity.Id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.InternalServerError, "Create owner payout failed.");
            return DataResult.ResultSuccess(MapToDto(created), "Insert success!", statusCode: 201);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create owner payout");
            throw;
        }
    }

    private static OwnerPayoutDto MapToDto(OwnerPayout e) => new()
    {
        Id = e.Id,
        OwnerId = e.OwnerId,
        OwnerName = e.Owner.FullName,
        PeriodFrom = e.PeriodFrom,
        PeriodTo = e.PeriodTo,
        GrossAmountVnd = e.GrossAmountVnd,
        PlatformFeeVnd = e.PlatformFeeVnd,
        NetAmountVnd = e.NetAmountVnd,
        PaidAt = e.PaidAt,
        BankAccount = e.BankAccount
    };
}
