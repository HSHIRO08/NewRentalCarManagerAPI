using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Models;

namespace NewRentalCarManagerAPI.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context) => _context = context;

    private IRepository<User>? _users;
    private IRepository<Car>? _cars;
    private IRepository<CarBrand>? _carBrands;
    private IRepository<CarModel>? _carModels;
    private IRepository<CarPricing>? _carPricings;
    private IRepository<CarAvailabilityBlock>? _carAvailabilityBlocks;
    private IRepository<Location>? _locations;
    private IRepository<Booking>? _bookings;
    private IRepository<Review>? _reviews;
    private IRepository<Promotion>? _promotions;
    private IRepository<Transaction>? _transactions;
    private IRepository<DamageReport>? _damageReports;
    private IRepository<Penalty>? _penalties;
    private IRepository<OwnerPayout>? _ownerPayouts;
    private IRepository<Role>? _roles;
    private IRepository<Permission>? _permissions;
    private IRepository<RefreshToken>? _refreshTokens;
    private IRepository<OtpToken>? _otpTokens;
    private IRepository<ExternalLogin>? _externalLogins;
    private IRepository<ApiKey>? _apiKeys;
    private IRepository<NotificationLog>? _notificationLogs;

    public IRepository<User> Users => _users ??= new Repository<User>(_context);
    public IRepository<Car> Cars => _cars ??= new Repository<Car>(_context);
    public IRepository<CarBrand> CarBrands => _carBrands ??= new Repository<CarBrand>(_context);
    public IRepository<CarModel> CarModels => _carModels ??= new Repository<CarModel>(_context);
    public IRepository<CarPricing> CarPricings => _carPricings ??= new Repository<CarPricing>(_context);
    public IRepository<CarAvailabilityBlock> CarAvailabilityBlocks => _carAvailabilityBlocks ??= new Repository<CarAvailabilityBlock>(_context);
    public IRepository<Location> Locations => _locations ??= new Repository<Location>(_context);
    public IRepository<Booking> Bookings => _bookings ??= new Repository<Booking>(_context);
    public IRepository<Review> Reviews => _reviews ??= new Repository<Review>(_context);
    public IRepository<Promotion> Promotions => _promotions ??= new Repository<Promotion>(_context);
    public IRepository<Transaction> Transactions => _transactions ??= new Repository<Transaction>(_context);
    public IRepository<DamageReport> DamageReports => _damageReports ??= new Repository<DamageReport>(_context);
    public IRepository<Penalty> Penalties => _penalties ??= new Repository<Penalty>(_context);
    public IRepository<OwnerPayout> OwnerPayouts => _ownerPayouts ??= new Repository<OwnerPayout>(_context);
    public IRepository<Role> Roles => _roles ??= new Repository<Role>(_context);
    public IRepository<Permission> Permissions => _permissions ??= new Repository<Permission>(_context);
    public IRepository<RefreshToken> RefreshTokens => _refreshTokens ??= new Repository<RefreshToken>(_context);
    public IRepository<OtpToken> OtpTokens => _otpTokens ??= new Repository<OtpToken>(_context);
    public IRepository<ExternalLogin> ExternalLogins => _externalLogins ??= new Repository<ExternalLogin>(_context);
    public IRepository<ApiKey> ApiKeys => _apiKeys ??= new Repository<ApiKey>(_context);
    public IRepository<NotificationLog> NotificationLogs => _notificationLogs ??= new Repository<NotificationLog>(_context);

    public async Task BeginTransactionAsync() => await _context.Database.BeginTransactionAsync();

    public async Task CommitTransactionAsync() => await _context.Database.CommitTransactionAsync();

    public async Task RollbackTransactionAsync() => await _context.Database.RollbackTransactionAsync();

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
