namespace NewRentalCarManagerAPI.Application.Features.Payments;

/// <summary>Parsed result from VNPay return/IPN query string.</summary>
public record VnPayReturnResult(
    bool IsSignatureValid,
    bool IsSuccess,
    string ResponseCode,
    string TxnRef,
    string GatewayTxId,
    int AmountVnd);

/// <summary>Request body to create a VNPay payment URL.</summary>
public class CreateVnPayUrlRequest
{
    public Guid BookingId { get; set; }
}

/// <summary>Response containing the redirect URL and idempotency ref.</summary>
public class CreateVnPayUrlResponse
{
    public string PaymentUrl { get; set; } = null!;
    public string TxnRef { get; set; } = null!;
}
