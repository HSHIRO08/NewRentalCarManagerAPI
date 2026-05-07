using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewRentalCarManagerAPI.Common;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Models;
using System.Net;

namespace NewRentalCarManagerAPI.Application.Features.Payments;

public class TransactionService 
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(IUnitOfWork uow, ILogger<TransactionService> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    private IQueryable<Transaction> BaseQuery() => _uow.Transactions.Query()
        .Include(t => t.Payer);

    public async Task<DataResult> GetByBookingAsync(Guid bookingId, PaymentListInput input)
    {
        try
        {
            var query = BaseQuery().Where(t => t.BookingId == bookingId).OrderByDescending(t => t.CreatedAt);
            var totalCount = await query.CountAsync();
            var items = await query.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();
            return DataResult.ResultSuccess(items.Select(MapToDto).ToList(), "Get success!", totalCount);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get transactions by booking {BookingId}", bookingId);
            throw;
        }
    }

    public async Task<DataResult> GetByPayerAsync(Guid payerId, PaymentListInput input)
    {
        try
        {
            var query = BaseQuery().Where(t => t.PayerId == payerId).OrderByDescending(t => t.CreatedAt);
            var totalCount = await query.CountAsync();
            var items = await query.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();
            return DataResult.ResultSuccess(items.Select(MapToDto).ToList(), "Get success!", totalCount);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get transactions by payer {PayerId}", payerId);
            throw;
        }
    }

    public async Task<DataResult> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await BaseQuery().FirstOrDefaultAsync(t => t.Id == id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Transaction not found!");
            return DataResult.ResultSuccess(MapToDto(entity), "Get success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get transaction {Id}", id);
            throw;
        }
    }

    public async Task<DataResult> CreateAsync(CreateTransactionDto dto)
    {
        try
        {
            var entity = new Transaction
            {
                BookingId = dto.BookingId,
                PayerId = dto.PayerId,
                AmountVnd = dto.AmountVnd,
                GatewayTxId = dto.GatewayTxId,
                IdempotencyKey = dto.IdempotencyKey,
                Note = dto.Note,
                CreatedAt = DateTime.UtcNow
            };
            await _uow.Transactions.AddAsync(entity);
            var created = await BaseQuery().FirstOrDefaultAsync(t => t.Id == entity.Id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.InternalServerError, "Create transaction failed.");
            return DataResult.ResultSuccess(MapToDto(created), "Insert success!", statusCode: 201);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create transaction");
            throw;
        }
    }

    private static TransactionDto MapToDto(Transaction e) => new()
    {
        Id = e.Id,
        BookingId = e.BookingId,
        PayerId = e.PayerId,
        PayerName = e.Payer.FullName,
        PayoutId = e.PayoutId,
        AmountVnd = e.AmountVnd,
        GatewayTxId = e.GatewayTxId,
        Note = e.Note,
        PaidAt = e.PaidAt,
        CreatedAt = e.CreatedAt
    };
}
