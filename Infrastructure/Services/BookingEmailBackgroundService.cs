using System.Threading.Channels;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Enums;
using NewRentalCarManagerAPI.Models;

namespace NewRentalCarManagerAPI.Infrastructure.Services;

public record BookingEmailJob(
    Guid BookingId,
    Guid RenterId,
    string RenterEmail,
    string RenterName,
    string CarName,
    DateTime RentalStart,
    DateTime RentalEnd,
    int TotalPriceVnd,
    DateTime PaidAt);

public interface IBookingEmailQueue
{
    ValueTask EnqueueAsync(BookingEmailJob job);
}

public class BookingEmailBackgroundService : BackgroundService, IBookingEmailQueue
{
    private readonly Channel<BookingEmailJob> _channel = Channel.CreateUnbounded<BookingEmailJob>();
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BookingEmailBackgroundService> _logger;

    public BookingEmailBackgroundService(IServiceScopeFactory scopeFactory, ILogger<BookingEmailBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public ValueTask EnqueueAsync(BookingEmailJob job)
    {
        return _channel.Writer.WriteAsync(job);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var job in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var emailData = new BookingEmailData
                {
                    BookingId = job.BookingId.ToString(),
                    CarName = job.CarName,
                    RentalStart = job.RentalStart.ToString("dd/MM/yyyy"),
                    RentalEnd = job.RentalEnd.ToString("dd/MM/yyyy"),
                    TotalPrice = $"{job.TotalPriceVnd:N0} đ",
                    PaidAt = job.PaidAt
                };

                var sent = await emailService.SendBookingConfirmationAsync(job.RenterEmail, job.RenterName, emailData);

                var log = new NotificationLog
                {
                    UserId = job.RenterId,
                    EventType = "booking_payment_confirmed",
                    Channel = NotifChannel.Email,
                    Recipient = job.RenterEmail,
                    Payload = $"Booking #{job.BookingId} payment confirmed. Total: {job.TotalPriceVnd:N0} đ",
                    Status = sent ? NotifStatus.Sent : NotifStatus.Failed,
                    SentAt = sent ? job.PaidAt : null,
                    ErrorMsg = sent ? null : "SMTP send failed",
                    CreatedAt = job.PaidAt
                };
                await uow.NotificationLogs.AddAsync(log);
                await uow.SaveChangesAsync();

                _logger.LogInformation("Booking email for {BookingId}: {Status}", job.BookingId, sent ? "Sent" : "Failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process booking email job for {BookingId}", job.BookingId);
            }
        }
    }
}
