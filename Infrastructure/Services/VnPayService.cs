using System.Security.Cryptography;
using System.Text;
using System.Web;
using NewRentalCarManagerAPI.Application.Features.Payments;

namespace NewRentalCarManagerAPI.Infrastructure.Services;

public interface IVnPayService
{
    string CreatePaymentUrl(Guid bookingId, long amountVnd, string orderInfo, string clientIp);
    VnPayReturnResult ProcessReturn(IQueryCollection query);
}

/// <summary>
/// Integrates with VNPay payment gateway v2.1.
/// Configuration keys (appsettings.json): VnPay:TmnCode, VnPay:HashSecret,
/// VnPay:PaymentUrl, VnPay:ReturnUrl.
/// </summary>
public class VnPayService : IVnPayService
{
    private readonly IConfiguration _config;

    public VnPayService(IConfiguration config) => _config = config;

    private string TmnCode   => _config["VnPay:TmnCode"]!;
    private string HashSecret => _config["VnPay:HashSecret"]!;
    private string PaymentUrl => _config["VnPay:PaymentUrl"]!;
    private string ReturnUrl  => _config["VnPay:ReturnUrl"]!;

    /// <summary>
    /// Builds a signed VNPay redirect URL. The TxnRef is the booking GUID so the
    /// return handler can look up the booking without extra state.
    /// </summary>
    public string CreatePaymentUrl(Guid bookingId, long amountVnd, string orderInfo, string clientIp)
    {
        // VNPay expects amount in the smallest unit (× 100 for VND)
        var vnpParams = new SortedDictionary<string, string>(StringComparer.Ordinal)
        {
            ["vnp_Version"]    = "2.1.0",
            ["vnp_Command"]    = "pay",
            ["vnp_TmnCode"]    = TmnCode,
            ["vnp_Amount"]     = (amountVnd * 100).ToString(),
            ["vnp_CreateDate"] = DateTime.Now.ToString("yyyyMMddHHmmss"),
            ["vnp_CurrCode"]   = "VND",
            ["vnp_IpAddr"]     = clientIp,
            ["vnp_Locale"]     = "vn",
            ["vnp_OrderInfo"]  = orderInfo,
            ["vnp_OrderType"]  = "other",
            ["vnp_ReturnUrl"]  = ReturnUrl,
            ["vnp_TxnRef"]     = bookingId.ToString(),
            ["vnp_ExpireDate"] = DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss"),
        };

        // Build URL query string (percent-encoded values)
        var queryStr  = string.Join("&", vnpParams.Select(kv => $"{kv.Key}={HttpUtility.UrlEncode(kv.Value)}"));
        // Sign data uses raw (non-encoded) values
        var signData  = string.Join("&", vnpParams.Select(kv => $"{kv.Key}={kv.Value}"));
        var secureHash = HmacSha512(HashSecret, signData);

        return $"{PaymentUrl}?{queryStr}&vnp_SecureHash={secureHash}";
    }

    /// <summary>
    /// Validates the HMAC-SHA512 signature in the VNPay return/IPN query string and
    /// returns a strongly-typed result.
    /// </summary>
    public VnPayReturnResult ProcessReturn(IQueryCollection query)
    {
        // Collect all vnp_ params except the hash itself, then sort alphabetically
        var vnpParams = query
            .Where(kv => kv.Key.StartsWith("vnp_", StringComparison.OrdinalIgnoreCase)
                         && !kv.Key.Equals("vnp_SecureHash", StringComparison.OrdinalIgnoreCase))
            .OrderBy(kv => kv.Key, StringComparer.Ordinal)
            .ToDictionary(kv => kv.Key, kv => kv.Value.ToString());

        var signData      = string.Join("&", vnpParams.Select(kv => $"{kv.Key}={kv.Value}"));
        var expectedHash  = HmacSha512(HashSecret, signData);
        var receivedHash  = query["vnp_SecureHash"].ToString();
        var isValid       = string.Equals(expectedHash, receivedHash, StringComparison.OrdinalIgnoreCase);

        var responseCode  = query["vnp_ResponseCode"].ToString();
        var txnRef        = query["vnp_TxnRef"].ToString();
        var gatewayTxId   = query["vnp_TransactionNo"].ToString();
        var amountRaw     = query["vnp_Amount"].ToString();
        var amountVnd     = long.TryParse(amountRaw, out var a) ? (int)(a / 100) : 0;

        return new VnPayReturnResult(
            IsSignatureValid: isValid,
            IsSuccess:        isValid && responseCode == "00",
            ResponseCode:     responseCode,
            TxnRef:           txnRef,
            GatewayTxId:      gatewayTxId,
            AmountVnd:        amountVnd);
    }

    private static string HmacSha512(string key, string data)
    {
        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hash).ToLower();
    }
}
