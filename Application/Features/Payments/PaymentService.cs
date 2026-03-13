using Microsoft.EntityFrameworkCore;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Models;

namespace NewRentalCarManagerAPI.Application.Features.Payments;

// ══════════════════════════════════════════════
//  Transaction
// ══════════════════════════════════════════════
public interface ITransactionService
{
    Task<IEnumerable<TransactionDto>> GetByBookingAsync(Guid bookingId);
    Task<IEnumerable<TransactionDto>> GetByPayerAsync(Guid payerId);
    Task<TransactionDto?> GetByIdAsync(Guid id);
    Task<TransactionDto> CreateAsync(CreateTransactionDto dto);
}

public class TransactionService : ITransactionService
{
    private readonly IUnitOfWork _uow;
    public TransactionService(IUnitOfWork uow) => _uow = uow;

    private IQueryable<Transaction> BaseQuery() => _uow.Transactions.Query()
        .Include(t => t.Payer);

    public async Task<IEnumerable<TransactionDto>> GetByBookingAsync(Guid bookingId)
    {
        var items = await BaseQuery().Where(t => t.BookingId == bookingId)
            .OrderByDescending(t => t.CreatedAt).ToListAsync();
        return items.Select(MapToDto);
    }

    public async Task<IEnumerable<TransactionDto>> GetByPayerAsync(Guid payerId)
    {
        var items = await BaseQuery().Where(t => t.PayerId == payerId)
            .OrderByDescending(t => t.CreatedAt).ToListAsync();
        return items.Select(MapToDto);
    }

    public async Task<TransactionDto?> GetByIdAsync(Guid id)
    {
        var e = await BaseQuery().FirstOrDefaultAsync(t => t.Id == id);
        return e is null ? null : MapToDto(e);
    }

    public async Task<TransactionDto> CreateAsync(CreateTransactionDto dto)
    {
        var e = new Transaction
        {
            BookingId = dto.BookingId, PayerId = dto.PayerId,
            AmountVnd = dto.AmountVnd, GatewayTxId = dto.GatewayTxId,
            IdempotencyKey = dto.IdempotencyKey, Note = dto.Note,
            CreatedAt = DateTime.UtcNow
        };
        await _uow.Transactions.AddAsync(e);
        return (await GetByIdAsync(e.Id))!;
    }

    private static TransactionDto MapToDto(Transaction e) => new()
    {
        Id = e.Id, BookingId = e.BookingId, PayerId = e.PayerId,
        PayerName = e.Payer.FullName, PayoutId = e.PayoutId,
        AmountVnd = e.AmountVnd, GatewayTxId = e.GatewayTxId,
        Note = e.Note, PaidAt = e.PaidAt, CreatedAt = e.CreatedAt
    };
}

// ══════════════════════════════════════════════
//  OwnerPayout
// ══════════════════════════════════════════════
public interface IOwnerPayoutService
{
    Task<IEnumerable<OwnerPayoutDto>> GetByOwnerAsync(Guid ownerId);
    Task<OwnerPayoutDto?> GetByIdAsync(Guid id);
    Task<OwnerPayoutDto> CreateAsync(CreateOwnerPayoutDto dto);
}

public class OwnerPayoutService : IOwnerPayoutService
{
    private readonly IUnitOfWork _uow;
    public OwnerPayoutService(IUnitOfWork uow) => _uow = uow;

    private IQueryable<OwnerPayout> BaseQuery() => _uow.OwnerPayouts.Query()
        .Include(p => p.Owner);

    public async Task<IEnumerable<OwnerPayoutDto>> GetByOwnerAsync(Guid ownerId)
    {
        var items = await BaseQuery().Where(p => p.OwnerId == ownerId)
            .OrderByDescending(p => p.PeriodTo).ToListAsync();
        return items.Select(MapToDto);
    }

    public async Task<OwnerPayoutDto?> GetByIdAsync(Guid id)
    {
        var e = await BaseQuery().FirstOrDefaultAsync(p => p.Id == id);
        return e is null ? null : MapToDto(e);
    }

    public async Task<OwnerPayoutDto> CreateAsync(CreateOwnerPayoutDto dto)
    {
        var e = new OwnerPayout
        {
            OwnerId = dto.OwnerId, PeriodFrom = dto.PeriodFrom, PeriodTo = dto.PeriodTo,
            GrossAmountVnd = dto.GrossAmountVnd, PlatformFeeVnd = dto.PlatformFeeVnd,
            NetAmountVnd = dto.GrossAmountVnd - dto.PlatformFeeVnd, BankAccount = dto.BankAccount
        };
        await _uow.OwnerPayouts.AddAsync(e);
        return (await GetByIdAsync(e.Id))!;
    }

    private static OwnerPayoutDto MapToDto(OwnerPayout e) => new()
    {
        Id = e.Id, OwnerId = e.OwnerId, OwnerName = e.Owner.FullName,
        PeriodFrom = e.PeriodFrom, PeriodTo = e.PeriodTo,
        GrossAmountVnd = e.GrossAmountVnd, PlatformFeeVnd = e.PlatformFeeVnd,
        NetAmountVnd = e.NetAmountVnd, PaidAt = e.PaidAt, BankAccount = e.BankAccount
    };
}
