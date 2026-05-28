using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewRentalCarManagerAPI.Common;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Models;
using System.Net;

namespace NewRentalCarManagerAPI.Application.Features.Payments;

public class TransactionService 
{
    private readonly IRepository<Transaction> _transactionRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(IRepository<Transaction> transactionRepository, IMapper mapper, ILogger<TransactionService> logger)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
        _logger = logger;
    }

    private IQueryable<Transaction> BaseQuery() => _transactionRepository.Query()
        .Include(t => t.Payer);

    public async Task<DataResult> GetByBookingAsync(Guid bookingId, PaymentListInput input)
    {
        try
        {
            var query = BaseQuery().Where(t => t.BookingId == bookingId).OrderByDescending(t => t.CreatedAt);
            var totalCount = await query.CountAsync();
            var items = await query.Skip(input.SkipCount).Take(input.MaxResultCount)
                .ProjectTo<TransactionDto>(_mapper.ConfigurationProvider).ToListAsync();
            return DataResult.ResultSuccess(items, "Get success!", totalCount);
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
            var items = await query.Skip(input.SkipCount).Take(input.MaxResultCount)
                .ProjectTo<TransactionDto>(_mapper.ConfigurationProvider).ToListAsync();
            return DataResult.ResultSuccess(items, "Get success!", totalCount);
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
            return DataResult.ResultSuccess(_mapper.Map<TransactionDto>(entity), "Get success!");
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
            await _transactionRepository.AddAsync(entity);
            var created = await BaseQuery().FirstOrDefaultAsync(t => t.Id == entity.Id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.InternalServerError, "Create transaction failed.");
            return DataResult.ResultSuccess(_mapper.Map<TransactionDto>(created), "Insert success!", statusCode: 201);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create transaction");
            throw;
        }
    }

}
