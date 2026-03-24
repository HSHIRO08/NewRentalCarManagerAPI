using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NewRentalCarManagerAPI.Application.Features.Auth;
using NewRentalCarManagerAPI.Application.Features.Bookings;
using NewRentalCarManagerAPI.Application.Features.Fleet;
using NewRentalCarManagerAPI.Application.Features.Ops;
using NewRentalCarManagerAPI.Application.Features.Payments;
using NewRentalCarManagerAPI.Application.Features.Users;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Infrastructure.Authorization;
using NewRentalCarManagerAPI.Infrastructure.Persistence;
using NewRentalCarManagerAPI.Infrastructure.Services;
using NewRentalCarManagerAPI.Middleware;
using NewRentalCarManagerAPI.Enums;
using NewRentalCarManagerAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// ── Database with PostgreSQL native enum mapping ──
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
var dataSourceBuilder = new Npgsql.NpgsqlDataSourceBuilder(connectionString);

dataSourceBuilder.MapEnum<BookingStatus>("booking.booking_status");
dataSourceBuilder.MapEnum<PromotionType>("booking.promotion_type");
dataSourceBuilder.MapEnum<SagaStatus>("booking.saga_status");
dataSourceBuilder.MapEnum<CarStatus>("fleet.car_status");
dataSourceBuilder.MapEnum<FuelType>("fleet.fuel_type");
dataSourceBuilder.MapEnum<RentalType>("fleet.rental_type");
dataSourceBuilder.MapEnum<TransmissionType>("fleet.transmission_type");
dataSourceBuilder.MapEnum<OtpPurpose>("identity.otp_purpose");
dataSourceBuilder.MapEnum<ProviderName>("identity.provider_name");
dataSourceBuilder.MapEnum<UserStatus>("identity.user_status");
dataSourceBuilder.MapEnum<NotifChannel>("notification.notif_channel");
dataSourceBuilder.MapEnum<NotifStatus>("notification.notif_status");
dataSourceBuilder.MapEnum<DamageSeverity>("ops.damage_severity");
dataSourceBuilder.MapEnum<DamageStatus>("ops.damage_status");
dataSourceBuilder.MapEnum<PenaltyStatus>("ops.penalty_status");
dataSourceBuilder.MapEnum<PenaltyType>("ops.penalty_type");
dataSourceBuilder.MapEnum<PaymentDirection>("payment.payment_direction");
dataSourceBuilder.MapEnum<PaymentMethod>("payment.payment_method");
dataSourceBuilder.MapEnum<PaymentStatus>("payment.payment_status");

var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(dataSource));

// ── Unit of Work ──
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ── Infrastructure Services ──
builder.Services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddSingleton<ITokenService, JwtTokenService>();
builder.Services.AddSingleton<ISlugService, SlugService>();

// ── Authorization ──
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddAuthorization(opt =>
{
    opt.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// ── Application Services ──
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICarBrandService, CarBrandService>();
builder.Services.AddScoped<ICarModelService, CarModelService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<ICarPricingService, CarPricingService>();
builder.Services.AddScoped<ICarAvailabilityBlockService, CarAvailabilityBlockService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IDamageReportService, DamageReportService>();
builder.Services.AddScoped<IPenaltyService, PenaltyService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IOwnerPayoutService, OwnerPayoutService>();

// ── JWT Authentication ──
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddCors(opt =>
    opt.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Nhập access token vào đây."
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.SeedDefaultRolesAndPermissions();

// ── Middleware Pipeline ──
app.UseCors("AllowAll");
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<UnitOfWorkCommitMiddleware>();
app.MapControllers();

app.Run();
