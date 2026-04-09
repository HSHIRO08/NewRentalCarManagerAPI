using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace NewRentalCarManagerAPI.Infrastructure.Services;

public interface IEmailService
{
    Task<bool> SendBookingConfirmationAsync(string toEmail, string toName, BookingEmailData data);
}

public class BookingEmailData
{
    public string BookingId { get; set; } = null!;
    public string CarName { get; set; } = null!;
    public string RentalStart { get; set; } = null!;
    public string RentalEnd { get; set; } = null!;
    public string TotalPrice { get; set; } = null!;
    public DateTime PaidAt { get; set; }
}

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IConfiguration config, ILogger<SmtpEmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task<bool> SendBookingConfirmationAsync(string toEmail, string toName, BookingEmailData data)
    {
        try
        {
            var smtpSection = _config.GetSection("Smtp");
            var host = smtpSection["Host"]!;
            var port = int.Parse(smtpSection["Port"]!);
            var useSsl = bool.Parse(smtpSection["UseSsl"] ?? "true");
            var username = smtpSection["Username"]!;
            var password = smtpSection["Password"]!;
            var fromEmail = smtpSection["FromEmail"]!;
            var fromName = smtpSection["FromName"] ?? "AutoRent";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromEmail));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = $"Xác nhận thanh toán - Booking #{data.BookingId[..8]}";

            var body = $@"
<html>
<body style='font-family: Inter, Arial, sans-serif; color: #1f2230; max-width: 600px; margin: 0 auto;'>
  <div style='background: linear-gradient(135deg, #16a34a, #22c55e); padding: 30px; border-radius: 14px 14px 0 0; text-align: center;'>
    <h1 style='color: #fff; margin: 0; font-size: 24px;'>🚗 AutoRent</h1>
    <p style='color: rgba(255,255,255,0.85); margin: 8px 0 0;'>Xác nhận thanh toán thành công</p>
  </div>
  <div style='padding: 30px; border: 1px solid #e4e8f2; border-top: none; border-radius: 0 0 14px 14px;'>
    <p>Xin chào <strong>{toName}</strong>,</p>
    <p>Thanh toán cho đơn thuê xe của bạn đã được xác nhận thành công!</p>
    
    <div style='background: #f0fdf4; border: 1px solid #bbf7d0; border-radius: 10px; padding: 20px; margin: 20px 0;'>
      <h3 style='margin: 0 0 12px; color: #15803d;'>Chi tiết đơn thuê</h3>
      <table style='width: 100%; border-collapse: collapse;'>
        <tr><td style='padding: 6px 0; color: #61657a;'>Mã booking:</td><td style='padding: 6px 0; font-weight: 600;'>#{data.BookingId[..8]}</td></tr>
        <tr><td style='padding: 6px 0; color: #61657a;'>Xe:</td><td style='padding: 6px 0; font-weight: 600;'>{data.CarName}</td></tr>
        <tr><td style='padding: 6px 0; color: #61657a;'>Thời gian thuê:</td><td style='padding: 6px 0; font-weight: 600;'>{data.RentalStart} → {data.RentalEnd}</td></tr>
        <tr><td style='padding: 6px 0; color: #61657a;'>Tổng tiền:</td><td style='padding: 6px 0; font-weight: 700; color: #16a34a; font-size: 18px;'>{data.TotalPrice}</td></tr>
        <tr><td style='padding: 6px 0; color: #61657a;'>Thanh toán lúc:</td><td style='padding: 6px 0; font-weight: 600;'>{data.PaidAt:dd/MM/yyyy HH:mm}</td></tr>
      </table>
    </div>

    <p>Cảm ơn bạn đã sử dụng dịch vụ AutoRent! Chúc bạn có chuyến đi vui vẻ. 🎉</p>
    
    <hr style='border: none; border-top: 1px solid #e4e8f2; margin: 20px 0;' />
    <p style='font-size: 12px; color: #61657a;'>Email này được gửi tự động từ hệ thống AutoRent. Vui lòng không trả lời email này.</p>
  </div>
</body>
</html>";

            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync(host, port, useSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
            await client.AuthenticateAsync(username, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Booking confirmation email sent to {Email} for booking {BookingId}", toEmail, data.BookingId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send booking confirmation email to {Email}", toEmail);
            return false;
        }
    }
}
