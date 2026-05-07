using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewRentalCarManagerAPI.Common;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Enums;
using NewRentalCarManagerAPI.Models;
using System.Net;

namespace NewRentalCarManagerAPI.Application.Features.Payments;

public class OwnerPayoutService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<OwnerPayoutService> _logger;

    public OwnerPayoutService(IUnitOfWork uow, IMapper mapper, ILogger<OwnerPayoutService> logger)
    {
        _uow = uow;
        _mapper = mapper;
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
            var items = await query.Skip(input.SkipCount).Take(input.MaxResultCount)
                .ProjectTo<OwnerPayoutDto>(_mapper.ConfigurationProvider).ToListAsync();
            return DataResult.ResultSuccess(items, "Get success!", totalCount);
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
            return DataResult.ResultSuccess(_mapper.Map<OwnerPayoutDto>(entity), "Get success!");
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
            // Platform fee rate (20%). Move to config/appsettings when needed.
            const decimal platformFeeRate = 0.20m;

            // Convert DateOnly period to UTC DateTime boundaries for DB query
            var periodFromDt = dto.PeriodFrom.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var periodToDt   = dto.PeriodTo.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

            // Sum TotalPriceVnd of all Completed bookings owned by this user in the period
            // Booking.Car.OwnerId identifies ownership; RentalEnd marks when earnings are realised
            var grossAmountVnd = await _uow.Bookings.Query()
                .Include(b => b.Car)
                .Where(b =>
                    b.Car.OwnerId == dto.OwnerId &&
                    b.Status == BookingStatus.Completed &&
                    b.RentalEnd >= periodFromDt &&
                    b.RentalEnd <= periodToDt)
                .SumAsync(b => (long)b.TotalPriceVnd);

            if (grossAmountVnd == 0)
                throw new UserFriendlyException(
                    (int)HttpStatusCode.UnprocessableEntity,
                    "No completed bookings found for this owner in the selected period.");

            var platformFeeVnd = (long)Math.Round(grossAmountVnd * platformFeeRate);
            var netAmountVnd   = grossAmountVnd - platformFeeVnd;

            var entity = new OwnerPayout
            {
                OwnerId        = dto.OwnerId,
                PeriodFrom     = dto.PeriodFrom,
                PeriodTo       = dto.PeriodTo,
                GrossAmountVnd = (int)grossAmountVnd,
                PlatformFeeVnd = (int)platformFeeVnd,
                NetAmountVnd   = (int)netAmountVnd,
                BankAccount    = dto.BankAccount
            };
            await _uow.OwnerPayouts.AddAsync(entity);

            var created = await BaseQuery().FirstOrDefaultAsync(p => p.Id == entity.Id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.InternalServerError, "Create owner payout failed.");
            return DataResult.ResultSuccess(_mapper.Map<OwnerPayoutDto>(created), "Insert success!", statusCode: 201);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create owner payout");
            throw;
        }
    }

}
