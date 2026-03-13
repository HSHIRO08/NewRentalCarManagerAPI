using NewRentalCarManagerAPI.Models;

namespace NewRentalCarManagerAPI.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    IRepository<Car> Cars { get; }
    IRepository<CarBrand> CarBrands { get; }
    IRepository<CarModel> CarModels { get; }
    IRepository<CarPricing> CarPricings { get; }
    IRepository<CarAvailabilityBlock> CarAvailabilityBlocks { get; }
    IRepository<Location> Locations { get; }
    IRepository<Booking> Bookings { get; }
    IRepository<Review> Reviews { get; }
    IRepository<Promotion> Promotions { get; }
    IRepository<Transaction> Transactions { get; }
    IRepository<DamageReport> DamageReports { get; }
    IRepository<Penalty> Penalties { get; }
    IRepository<OwnerPayout> OwnerPayouts { get; }
    IRepository<Role> Roles { get; }
    IRepository<Permission> Permissions { get; }
    IRepository<RefreshToken> RefreshTokens { get; }
    IRepository<OtpToken> OtpTokens { get; }
    IRepository<ExternalLogin> ExternalLogins { get; }
    IRepository<ApiKey> ApiKeys { get; }
    IRepository<NotificationLog> NotificationLogs { get; }
    Task<int> SaveChangesAsync();
}
