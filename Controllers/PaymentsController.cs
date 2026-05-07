using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewRentalCarManagerAPI.Application.Features.Payments;
using NewRentalCarManagerAPI.Common;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Enums;
using NewRentalCarManagerAPI.Infrastructure.Services;
using NewRentalCarManagerAPI.Models;

namespace NewRentalCarManagerAPI.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IVnPayService _vnPay;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<PaymentsController> _logger;
    private readonly IConfiguration _config;

    public PaymentsController(
        IVnPayService vnPay,
        IUnitOfWork uow,
        ILogger<PaymentsController> logger,
        IConfiguration config)
    {
        _vnPay   = vnPay;
        _uow     = uow;
        _logger  = logger;
        _config  = config;
    }

    // ──────────────────────────────────────────────
    //  POST /api/payments/vnpay/create
    //  Authenticated renter creates a VNPay payment URL for their booking.
    // ──────────────────────────────────────────────
    [HttpPost("vnpay/create")]
    [Authorize(Roles = "renter,admin")]
    public async Task<IActionResult> CreateVnPayUrl([FromBody] CreateVnPayUrlRequest dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(userId, out var payerId))
            return Unauthorized(ApiResult<CreateVnPayUrlResponse>.Fail("Cannot identify user"));

        var booking = await _uow.Bookings.Query()
            .Include(b => b.Transactions)
            .FirstOrDefaultAsync(b => b.Id == dto.BookingId);

        if (booking is null)
            return NotFound(ApiResult<CreateVnPayUrlResponse>.Fail("Booking not found"));

        // Only the renter who owns the booking (or admin) may pay
        if (booking.RenterId != payerId && !User.IsInRole("admin"))
            return StatusCode(403, ApiResult<CreateVnPayUrlResponse>.Fail("Forbidden"));

        if (booking.Status == BookingStatus.Cancelled)
            return BadRequest(ApiResult<CreateVnPayUrlResponse>.Fail("Booking is cancelled"));

        // Idempotency: reject if already successfully paid
        var alreadyPaid = booking.Transactions
            .Any(t => t.Status == PaymentStatus.Success && t.Direction == PaymentDirection.Charge);
        if (alreadyPaid)
            return Conflict(ApiResult<CreateVnPayUrlResponse>.Fail("Booking is already paid"));

        var clientIp  = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "127.0.0.1";
        var orderInfo = $"Thanh toan booking {dto.BookingId}";
        var url       = _vnPay.CreatePaymentUrl(dto.BookingId, booking.TotalPriceVnd, orderInfo, clientIp);

        _logger.LogInformation("Created VNPay URL for booking {BookingId}, amount={Amount}", dto.BookingId, booking.TotalPriceVnd);

        return Ok(ApiResult<CreateVnPayUrlResponse>.Ok(new CreateVnPayUrlResponse
        {
            PaymentUrl = url,
            TxnRef     = dto.BookingId.ToString()
        }));
    }

    // ──────────────────────────────────────────────
    //  GET /api/payments/vnpay/return
    //  VNPay browser redirect after payment completion.
    //  Validates signature, updates DB, then redirects frontend.
    // ──────────────────────────────────────────────
    [HttpGet("vnpay/return")]
    [AllowAnonymous]
    public async Task<IActionResult> VnPayReturn()
    {
        var result = _vnPay.ProcessReturn(Request.Query);
        var frontendBase = _config["VnPay:FrontendBaseUrl"]?.TrimEnd('/') ?? "http://localhost:5173";

        if (!result.IsSignatureValid)
        {
            _logger.LogWarning("VNPay return: invalid signature, TxnRef={TxnRef}", result.TxnRef);
            return Redirect($"{frontendBase}/payment/status?status=failed&code={result.ResponseCode}");
        }

        if (!Guid.TryParse(result.TxnRef, out var bookingId))
            return Redirect($"{frontendBase}/payment/status?status=failed&code=INVALID_REF");

        await RecordPaymentAsync(bookingId, result);

        var status = result.IsSuccess ? "paid" : "failed";
        return Redirect($"{frontendBase}/payment/status?status={status}&bookingId={bookingId}&code={result.ResponseCode}");
    }

    // ──────────────────────────────────────────────
    //  GET /api/payments/vnpay/ipn
    //  VNPay server-to-server IPN notification.
    //  Must return JSON { RspCode, Message } per VNPay spec.
    // ──────────────────────────────────────────────
    [HttpGet("vnpay/ipn")]
    [AllowAnonymous]
    public async Task<IActionResult> VnPayIpn()
    {
        var result = _vnPay.ProcessReturn(Request.Query);

        if (!result.IsSignatureValid)
            return Ok(new { RspCode = "97", Message = "Invalid signature" });

        if (!Guid.TryParse(result.TxnRef, out var bookingId))
            return Ok(new { RspCode = "01", Message = "Order not found" });

        var booking = await _uow.Bookings.Query()
            .Include(b => b.Transactions)
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking is null)
            return Ok(new { RspCode = "01", Message = "Order not found" });

        // Idempotency: already confirmed
        var alreadyPaid = booking.Transactions
            .Any(t => t.Status == PaymentStatus.Success && t.Direction == PaymentDirection.Charge);
        if (alreadyPaid)
            return Ok(new { RspCode = "02", Message = "Order already confirmed" });

        await RecordPaymentAsync(bookingId, result);
        return Ok(new { RspCode = "00", Message = "Confirm Success" });
    }

    // ──────────────────────────────────────────────
    //  GET /api/payments/transactions/booking/{id}
    //  Returns all transactions for a booking (renter or admin).
    // ──────────────────────────────────────────────
    [HttpGet("transactions/booking/{bookingId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetTransactionsByBooking(Guid bookingId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(userId, out var requesterId))
            return Unauthorized(ApiResult<IEnumerable<TransactionDto>>.Fail("Cannot identify user"));

        var booking = await _uow.Bookings.Query()
            .Include(b => b.Transactions).ThenInclude(t => t.Payer)
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking is null)
            return NotFound(ApiResult<IEnumerable<TransactionDto>>.Fail("Booking not found"));

        if (booking.RenterId != requesterId && !User.IsInRole("admin"))
            return StatusCode(403, ApiResult<IEnumerable<TransactionDto>>.Fail("Forbidden"));

        var dtos = booking.Transactions.Select(t => new TransactionDto
        {
            Id           = t.Id,
            BookingId    = t.BookingId,
            PayerId      = t.PayerId,
            PayerName    = t.Payer?.FullName ?? "",
            PayoutId     = t.PayoutId,
            AmountVnd    = t.AmountVnd,
            GatewayTxId  = t.GatewayTxId,
            Note         = t.Note,
            PaidAt       = t.PaidAt,
            CreatedAt    = t.CreatedAt
        });

        return Ok(ApiResult<IEnumerable<TransactionDto>>.Ok(dtos));
    }

    // ──────────────────────────────────────────────
    //  Internal helper: record the payment outcome.
    //  Called by both Return and IPN handlers.
    // ──────────────────────────────────────────────
    private async Task RecordPaymentAsync(Guid bookingId, VnPayReturnResult result)
    {
        var booking = await _uow.Bookings.Query()
            .Include(b => b.Transactions)
            .FirstOrDefaultAsync(b => b.Id == bookingId);
        if (booking is null) return;

        // Guard against duplicate IPN calls
        var alreadyProcessed = booking.Transactions
            .Any(t => t.GatewayTxId == result.GatewayTxId
                      && !string.IsNullOrEmpty(result.GatewayTxId));
        if (alreadyProcessed)
        {
            _logger.LogInformation("Duplicate IPN ignored for GatewayTxId={GatewayTxId}", result.GatewayTxId);
            return;
        }

        var now = DateTime.UtcNow;
        var tx = new Transaction
        {
            BookingId    = bookingId,
            PayerId      = booking.RenterId,
            AmountVnd    = result.AmountVnd > 0 ? result.AmountVnd : booking.TotalPriceVnd,
            Direction    = PaymentDirection.Charge,
            Method       = PaymentMethod.VnPay,
            Status       = result.IsSuccess ? PaymentStatus.Success : PaymentStatus.Failed,
            GatewayTxId  = result.GatewayTxId,
            GatewayMeta  = $"vnpay_code={result.ResponseCode}",
            PaidAt       = result.IsSuccess ? now : null,
            CreatedAt    = now,
            Note         = result.IsSuccess
                               ? "VNPay thanh toán thành công"
                               : $"VNPay thất bại (mã {result.ResponseCode})"
        };
        await _uow.Transactions.AddAsync(tx);

        if (result.IsSuccess && booking.Status == BookingStatus.Pending)
        {
            booking.Status    = BookingStatus.Confirmed;
            booking.UpdatedAt = now;
        }

        await _uow.SaveChangesAsync();
        _logger.LogInformation("Payment recorded: BookingId={BookingId}, Success={Success}", bookingId, result.IsSuccess);
    }
}
