using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewRentalCarManagerAPI.Common;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Enums;
using NewRentalCarManagerAPI.Models;
using System.Net;

namespace NewRentalCarManagerAPI.Application.Features.Payments;

public interface IDepositService
{
    Task<DataResult> GetStatusAsync(Guid bookingId);
    Task<DataResult> ChargeAsync(Guid bookingId, Guid payerId);
    Task<DataResult> RefundAsync(Guid bookingId, Guid actorId, RefundDepositDto dto);
}

public class DepositService : IDepositService
{
    private const string DepositNote = "deposit";
    private const string DepositRefundNote = "deposit_refund";

    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<DepositService> _logger;

    public DepositService(IUnitOfWork uow, IMapper mapper, ILogger<DepositService> logger)
    {
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<DataResult> GetStatusAsync(Guid bookingId)
    {
        try
        {
            var booking = await _uow.Bookings.Query()
                .Include(b => b.Transactions)
                .FirstOrDefaultAsync(b => b.Id == bookingId)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Booking not found!");

            var status = BuildStatus(booking);
            return DataResult.ResultSuccess(status, "Success");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get deposit status for booking {BookingId}", bookingId);
            throw;
        }
    }

    public async Task<DataResult> ChargeAsync(Guid bookingId, Guid payerId)
    {
        try
        {
            var booking = await _uow.Bookings.Query()
                .Include(b => b.Transactions)
                .FirstOrDefaultAsync(b => b.Id == bookingId)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Booking not found!");

            if (booking.Status != BookingStatus.Confirmed)
                throw new UserFriendlyException((int)HttpStatusCode.Conflict,
                    $"Deposit chỉ thu được khi booking ở trạng thái Confirmed. Hiện tại: {booking.Status}");

            if (booking.DepositVnd <= 0)
                throw new UserFriendlyException((int)HttpStatusCode.BadRequest,
                    "Booking này không yêu cầu deposit.");

            var alreadyCharged = booking.Transactions.Any(t =>
                t.Note == DepositNote &&
                t.Direction == PaymentDirection.Charge &&
                t.Status == PaymentStatus.Success);

            if (alreadyCharged)
                throw new UserFriendlyException((int)HttpStatusCode.Conflict, "Deposit đã được thu.");

            var now = DateTime.UtcNow;
            var tx = new Transaction
            {
                BookingId = bookingId,
                PayerId = payerId,
                AmountVnd = booking.DepositVnd,
                Direction = PaymentDirection.Charge,
                Method = PaymentMethod.BankTransfer,
                Status = PaymentStatus.Success,
                Note = DepositNote,
                PaidAt = now,
                CreatedAt = now
            };
            await _uow.Transactions.AddAsync(tx);
            await _uow.SaveChangesAsync();

            return DataResult.ResultSuccess(BuildStatus(booking, tx), "Deposit thu thành công!", statusCode: 201);
        }
        catch (Exception e)
        {
            if (e is not UserFriendlyException)
                _logger.LogError(e, "Failed to charge deposit for booking {BookingId}", bookingId);
            throw;
        }
    }

    public async Task<DataResult> RefundAsync(Guid bookingId, Guid actorId, RefundDepositDto dto)
    {
        try
        {
            var booking = await _uow.Bookings.Query()
                .Include(b => b.Transactions)
                .FirstOrDefaultAsync(b => b.Id == bookingId)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Booking not found!");

            if (booking.Status != BookingStatus.Completed && booking.Status != BookingStatus.Cancelled)
                throw new UserFriendlyException((int)HttpStatusCode.Conflict,
                    "Deposit chỉ có thể hoàn khi booking đã Completed hoặc Cancelled.");

            var chargeTx = booking.Transactions.FirstOrDefault(t =>
                t.Note == DepositNote &&
                t.Direction == PaymentDirection.Charge &&
                t.Status == PaymentStatus.Success);

            if (chargeTx == null)
                throw new UserFriendlyException((int)HttpStatusCode.Conflict, "Chưa thu deposit — không thể hoàn.");

            var alreadyRefunded = booking.Transactions.Any(t =>
                t.Note == DepositRefundNote &&
                t.Direction == PaymentDirection.Refund &&
                t.Status == PaymentStatus.Success);

            if (alreadyRefunded)
                throw new UserFriendlyException((int)HttpStatusCode.Conflict, "Deposit đã được hoàn.");

            var refundAmount = dto.RefundAmountVnd ?? booking.DepositVnd;
            if (refundAmount <= 0 || refundAmount > booking.DepositVnd)
                throw new UserFriendlyException((int)HttpStatusCode.BadRequest,
                    $"Số tiền hoàn phải trong khoảng 1 – {booking.DepositVnd:N0} VND.");

            var now = DateTime.UtcNow;
            var tx = new Transaction
            {
                BookingId = bookingId,
                PayerId = actorId,
                AmountVnd = refundAmount,
                Direction = PaymentDirection.Refund,
                Method = PaymentMethod.BankTransfer,
                Status = PaymentStatus.Success,
                Note = DepositRefundNote + (dto.Reason != null ? $": {dto.Reason}" : string.Empty),
                PaidAt = now,
                CreatedAt = now
            };
            await _uow.Transactions.AddAsync(tx);
            await _uow.SaveChangesAsync();

            // Reload for accurate status
            booking.Transactions.Add(tx);
            return DataResult.ResultSuccess(BuildStatus(booking), "Deposit hoàn thành công!", statusCode: 201);
        }
        catch (Exception e)
        {
            if (e is not UserFriendlyException)
                _logger.LogError(e, "Failed to refund deposit for booking {BookingId}", bookingId);
            throw;
        }
    }

    // ── helpers ──────────────────────────────────────────────────
    private static DepositStatusDto BuildStatus(Booking booking, Transaction? extraChargeTx = null)
    {
        var allTx = booking.Transactions.Concat(extraChargeTx != null ? [extraChargeTx] : []);

        var chargeTx = allTx.FirstOrDefault(t =>
            t.Note == DepositNote &&
            t.Direction == PaymentDirection.Charge &&
            t.Status == PaymentStatus.Success);

        var refundTx = allTx.FirstOrDefault(t =>
            (t.Note != null && t.Note.StartsWith(DepositRefundNote)) &&
            t.Direction == PaymentDirection.Refund &&
            t.Status == PaymentStatus.Success);

        return new DepositStatusDto
        {
            BookingId = booking.Id,
            DepositVnd = booking.DepositVnd,
            IsCharged = chargeTx != null,
            IsRefunded = refundTx != null,
            RefundedAmountVnd = refundTx?.AmountVnd,
            ChargeTransactionId = chargeTx?.Id,
            RefundTransactionId = refundTx?.Id
        };
    }
}
