using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Enums;
using NewRentalCarManagerAPI.Infrastructure.Persistence;
using NewRentalCarManagerAPI.Models;

namespace NewRentalCarManagerAPI.Infrastructure.Services;

/// <summary>
/// Background service that runs every hour and raises a LateReturn Penalty
/// for any Active booking whose RentalEnd has passed and no Penalty record
/// of type LateReturn already exists.
/// </summary>
public class OverduePenaltyCronJob : BackgroundService
{
    // How long to wait between each scan
    private static readonly TimeSpan Interval = TimeSpan.FromHours(1);

    // Daily late-return penalty rate per day (or per started hour block)
    private const int PenaltyPerDayVnd = 200_000;

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OverduePenaltyCronJob> _logger;

    public OverduePenaltyCronJob(IServiceScopeFactory scopeFactory, ILogger<OverduePenaltyCronJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OverduePenaltyCronJob started.");

        // Delay first run by 5 minutes so app fully starts up
        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ScanAndPenalizeAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "OverduePenaltyCronJob scan failed.");
            }

            await Task.Delay(Interval, stoppingToken);
        }

        _logger.LogInformation("OverduePenaltyCronJob stopped.");
    }

    private async Task ScanAndPenalizeAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var bookingRepository = scope.ServiceProvider.GetRequiredService<IRepository<Booking>>();
        var penaltyRepository = scope.ServiceProvider.GetRequiredService<IRepository<Penalty>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var now = DateTime.UtcNow;

        // Find Active bookings past their RentalEnd that have no LateReturn penalty yet
        var overdueBookings = await bookingRepository.Query()
            .Include(b => b.Penalties)
            .Where(b =>
                b.Status == BookingStatus.Active &&
                b.RentalEnd < now &&
                !b.Penalties.Any(p => p.Type == PenaltyType.LateReturn))
            .ToListAsync(ct);

        if (overdueBookings.Count == 0)
            return;

        _logger.LogInformation("OverduePenaltyCronJob: {Count} overdue booking(s) found.", overdueBookings.Count);

        foreach (var booking in overdueBookings)
        {
            try
            {
                var overdueDays = Math.Ceiling((now - booking.RentalEnd).TotalDays);
                var penaltyAmount = (int)(overdueDays * PenaltyPerDayVnd);

                var penalty = new Penalty
                {
                    BookingId = booking.Id,
                    ChargedTo = booking.RenterId,
                    AmountVnd = penaltyAmount,
                    Type = PenaltyType.LateReturn,
                    Status = PenaltyStatus.Pending,
                    Description = $"Trả xe trễ {overdueDays:N0} ngày (tự động tính lúc {now:dd/MM/yyyy HH:mm} UTC).",
                    CreatedAt = now
                };
                await penaltyRepository.AddAsync(penalty);

                _logger.LogInformation(
                    "Penalty created for booking {BookingId}: {Days} day(s) late, {Amount:N0} VND.",
                    booking.Id, overdueDays, penaltyAmount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create penalty for booking {BookingId}.", booking.Id);
            }
        }

        await dbContext.SaveChangesAsync(ct);
    }
}
