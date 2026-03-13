using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NewRentalCarManagerAPI.Enums;

namespace NewRentalCarManagerAPI.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ApiKey> ApiKeys { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Car> Cars { get; set; }

    public virtual DbSet<CarAvailabilityBlock> CarAvailabilityBlocks { get; set; }

    public virtual DbSet<CarBrand> CarBrands { get; set; }

    public virtual DbSet<CarModel> CarModels { get; set; }

    public virtual DbSet<CarPricing> CarPricings { get; set; }

    public virtual DbSet<DamageReport> DamageReports { get; set; }

    public virtual DbSet<ExternalLogin> ExternalLogins { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<NotificationLog> NotificationLogs { get; set; }

    public virtual DbSet<OtpToken> OtpTokens { get; set; }

    public virtual DbSet<OwnerPayout> OwnerPayouts { get; set; }

    public virtual DbSet<Penalty> Penalties { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=RentalCarv2.2;Username=postgres;Password=admin2312");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum<BookingStatus>("booking", "booking_status")
            .HasPostgresEnum<PromotionType>("booking", "promotion_type")
            .HasPostgresEnum<SagaStatus>("booking", "saga_status")
            .HasPostgresEnum<CarStatus>("fleet", "car_status")
            .HasPostgresEnum<FuelType>("fleet", "fuel_type")
            .HasPostgresEnum<RentalType>("fleet", "rental_type")
            .HasPostgresEnum<TransmissionType>("fleet", "transmission_type")
            .HasPostgresEnum<OtpPurpose>("identity", "otp_purpose")
            .HasPostgresEnum<ProviderName>("identity", "provider_name")
            .HasPostgresEnum<UserStatus>("identity", "user_status")
            .HasPostgresEnum<NotifChannel>("notification", "notif_channel")
            .HasPostgresEnum<NotifStatus>("notification", "notif_status")
            .HasPostgresEnum<DamageSeverity>("ops", "damage_severity")
            .HasPostgresEnum<DamageStatus>("ops", "damage_status")
            .HasPostgresEnum<PenaltyStatus>("ops", "penalty_status")
            .HasPostgresEnum<PenaltyType>("ops", "penalty_type")
            .HasPostgresEnum<PaymentDirection>("payment", "payment_direction")
            .HasPostgresEnum<PaymentMethod>("payment", "payment_method")
            .HasPostgresEnum<PaymentStatus>("payment", "payment_status")
            .HasPostgresExtension("pgcrypto");

        modelBuilder.Entity<ApiKey>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("api_keys_pkey");

            entity.ToTable("api_keys", "identity");

            entity.HasIndex(e => e.KeyHash, "api_keys_key_hash_key").IsUnique();

            entity.HasIndex(e => e.UserId, "api_keys_user_id_idx").HasFilter("(revoked_at IS NULL)");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.KeyHash).HasColumnName("key_hash");
            entity.Property(e => e.KeyPrefix)
                .HasMaxLength(10)
                .HasColumnName("key_prefix");
            entity.Property(e => e.LastUsedAt).HasColumnName("last_used_at");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.RevokedAt).HasColumnName("revoked_at");
            entity.Property(e => e.Scopes)
                .HasDefaultValueSql("'{}'::text[]")
                .HasColumnName("scopes");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.ApiKeys)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("api_keys_user_id_fkey");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bookings_pkey");

            entity.ToTable("bookings", "booking");

            entity.HasIndex(e => new { e.CarId, e.RentalStart, e.RentalEnd }, "bookings_car_id_rental_start_rental_end_idx");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.ActualReturnAt).HasColumnName("actual_return_at");
            entity.Property(e => e.BasePriceVnd).HasColumnName("base_price_vnd");
            entity.Property(e => e.CancelReason).HasColumnName("cancel_reason");
            entity.Property(e => e.CarId).HasColumnName("car_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.DeliveryFeeVnd)
                .HasDefaultValue(0)
                .HasColumnName("delivery_fee_vnd");
            entity.Property(e => e.DepositVnd).HasColumnName("deposit_vnd");
            entity.Property(e => e.DiscountVnd)
                .HasDefaultValue(0)
                .HasColumnName("discount_vnd");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.PickupLocationId).HasColumnName("pickup_location_id");
            entity.Property(e => e.PricingId).HasColumnName("pricing_id");
            entity.Property(e => e.PromotionId).HasColumnName("promotion_id");
            entity.Property(e => e.RentalEnd).HasColumnName("rental_end");
            entity.Property(e => e.RentalStart).HasColumnName("rental_start");
            entity.Property(e => e.RenterId).HasColumnName("renter_id");
            entity.Property(e => e.ReturnLocationId).HasColumnName("return_location_id");
            entity.Property(e => e.SagaContext)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("saga_context");
            entity.Property(e => e.SagaStep)
                .HasMaxLength(50)
                .HasColumnName("saga_step");
            entity.Property(e => e.SurchargeVnd)
                .HasDefaultValue(0)
                .HasColumnName("surcharge_vnd");
            entity.Property(e => e.TotalPriceVnd).HasColumnName("total_price_vnd");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
            entity.Property(e => e.VatVnd)
                .HasDefaultValue(0)
                .HasColumnName("vat_vnd");
            entity.Property(e => e.Status)
                .HasColumnName("status");
            entity.Property(e => e.SagaStatusEnum)
                .HasColumnName("saga_status");

            entity.HasOne(d => d.Car).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("bookings_car_id_fkey");

            entity.HasOne(d => d.PickupLocation).WithMany(p => p.BookingPickupLocations)
                .HasForeignKey(d => d.PickupLocationId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("bookings_pickup_location_id_fkey");

            entity.HasOne(d => d.Pricing).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.PricingId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("bookings_pricing_id_fkey");

            entity.HasOne(d => d.Promotion).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.PromotionId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("bookings_promotion_id_fkey");

            entity.HasOne(d => d.Renter).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.RenterId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("bookings_renter_id_fkey");

            entity.HasOne(d => d.ReturnLocation).WithMany(p => p.BookingReturnLocations)
                .HasForeignKey(d => d.ReturnLocationId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("bookings_return_location_id_fkey");
        });

        modelBuilder.Entity<Car>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cars_pkey");

            entity.ToTable("cars", "fleet");

            entity.HasIndex(e => e.Features, "cars_features_idx").HasMethod("gin");

            entity.HasIndex(e => e.LicensePlate, "cars_license_plate_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Color)
                .HasMaxLength(50)
                .HasColumnName("color");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Features)
                .HasDefaultValueSql("'{}'::text[]")
                .HasColumnName("features");
            entity.Property(e => e.HasIotDevice)
                .HasDefaultValue(false)
                .HasColumnName("has_iot_device");
            entity.Property(e => e.ImageUrls)
                .HasDefaultValueSql("'{}'::text[]")
                .HasColumnName("image_urls");
            entity.Property(e => e.IotDeviceId)
                .HasMaxLength(100)
                .HasColumnName("iot_device_id");
            entity.Property(e => e.LicensePlate)
                .HasMaxLength(20)
                .HasColumnName("license_plate");
            entity.Property(e => e.LocationId).HasColumnName("location_id");
            entity.Property(e => e.ManufactureYear).HasColumnName("manufacture_year");
            entity.Property(e => e.MileageKm)
                .HasDefaultValue(0)
                .HasColumnName("mileage_km");
            entity.Property(e => e.ModelId).HasColumnName("model_id");
            entity.Property(e => e.OwnerId).HasColumnName("owner_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
            entity.Property(e => e.Status)
                .HasColumnName("status");
            entity.Property(e => e.FuelType)
                .HasColumnName("fuel_type");
            entity.Property(e => e.TransmissionType)
                .HasColumnName("transmission_type");

            entity.HasOne(d => d.Location).WithMany(p => p.Cars)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("cars_location_id_fkey");

            entity.HasOne(d => d.Model).WithMany(p => p.Cars)
                .HasForeignKey(d => d.ModelId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("cars_model_id_fkey");

            entity.HasOne(d => d.Owner).WithMany(p => p.Cars)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("cars_owner_id_fkey");
        });

        modelBuilder.Entity<CarAvailabilityBlock>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("car_availability_blocks_pkey");

            entity.ToTable("car_availability_blocks", "fleet");

            entity.HasIndex(e => new { e.CarId, e.BlockedFrom, e.BlockedTo }, "car_availability_blocks_car_id_blocked_from_blocked_to_idx");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.BlockedFrom).HasColumnName("blocked_from");
            entity.Property(e => e.BlockedTo).HasColumnName("blocked_to");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CarId).HasColumnName("car_id");
            entity.Property(e => e.Reason).HasColumnName("reason");
            entity.Property(e => e.Source)
                .HasMaxLength(20)
                .HasDefaultValueSql("'owner'::character varying")
                .HasColumnName("source");

            entity.HasOne(d => d.Car).WithMany(p => p.CarAvailabilityBlocks)
                .HasForeignKey(d => d.CarId)
                .HasConstraintName("car_availability_blocks_car_id_fkey");
        });

        modelBuilder.Entity<CarBrand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("car_brands_pkey");

            entity.ToTable("car_brands", "fleet");

            entity.HasIndex(e => e.Name, "car_brands_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.LogoUrl).HasColumnName("logo_url");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<CarModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("car_models_pkey");

            entity.ToTable("car_models", "fleet");

            entity.HasIndex(e => new { e.BrandId, e.Name }, "car_models_brand_id_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.BrandId).HasColumnName("brand_id");
            entity.Property(e => e.CarType)
                .HasMaxLength(50)
                .HasColumnName("car_type");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.SeatCount).HasColumnName("seat_count");

            entity.HasOne(d => d.Brand).WithMany(p => p.CarModels)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("car_models_brand_id_fkey");
        });

        modelBuilder.Entity<CarPricing>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("car_pricing_pkey");

            entity.ToTable("car_pricing", "fleet");

            entity.HasIndex(e => e.CarId, "car_pricing_car_id_idx").HasFilter("(is_active = true)");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CarId).HasColumnName("car_id");
            entity.Property(e => e.DurationHours).HasColumnName("duration_hours");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.PriceVnd).HasColumnName("price_vnd");
            entity.Property(e => e.RentalType)
                .HasColumnName("rental_type");

            entity.HasOne(d => d.Car).WithMany(p => p.CarPricings)
                .HasForeignKey(d => d.CarId)
                .HasConstraintName("car_pricing_car_id_fkey");
        });

        modelBuilder.Entity<DamageReport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("damage_reports_pkey");

            entity.ToTable("damage_reports", "ops");

            entity.HasIndex(e => e.BookingId, "damage_reports_booking_id_idx");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ImageUrls)
                .HasDefaultValueSql("'{}'::text[]")
                .HasColumnName("image_urls");
            entity.Property(e => e.RepairCostVnd).HasColumnName("repair_cost_vnd");
            entity.Property(e => e.ReportedBy).HasColumnName("reported_by");
            entity.Property(e => e.ResolvedAt).HasColumnName("resolved_at");
            entity.Property(e => e.Severity)
                .HasColumnName("severity");
            entity.Property(e => e.Status)
                .HasColumnName("status");

            entity.HasOne(d => d.Booking).WithMany(p => p.DamageReports)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("damage_reports_booking_id_fkey");

            entity.HasOne(d => d.ReportedByNavigation).WithMany(p => p.DamageReports)
                .HasForeignKey(d => d.ReportedBy)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("damage_reports_reported_by_fkey");
        });

        modelBuilder.Entity<ExternalLogin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("external_logins_pkey");

            entity.ToTable("external_logins", "identity");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.AccessToken).HasColumnName("access_token");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.ProfileData)
                .HasColumnType("jsonb")
                .HasColumnName("profile_data");
            entity.Property(e => e.ProviderUid)
                .HasMaxLength(255)
                .HasColumnName("provider_uid");
            entity.Property(e => e.RefreshToken).HasColumnName("refresh_token");
            entity.Property(e => e.TokenExpiresAt).HasColumnName("token_expires_at");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Provider)
                .HasColumnName("provider_name");

            entity.HasOne(d => d.User).WithMany(p => p.ExternalLogins)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("external_logins_user_id_fkey");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("locations_pkey");

            entity.ToTable("locations", "fleet");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.District)
                .HasMaxLength(100)
                .HasColumnName("district");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Latitude)
                .HasPrecision(10, 7)
                .HasColumnName("latitude");
            entity.Property(e => e.Longitude)
                .HasPrecision(10, 7)
                .HasColumnName("longitude");
            entity.Property(e => e.Ward)
                .HasMaxLength(100)
                .HasColumnName("ward");
        });

        modelBuilder.Entity<NotificationLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("notification_logs_pkey");

            entity.ToTable("notification_logs", "notification");

            entity.HasIndex(e => new { e.UserId, e.CreatedAt }, "notification_logs_user_id_created_at_idx").IsDescending(false, true);

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.ErrorMsg).HasColumnName("error_msg");
            entity.Property(e => e.EventType)
                .HasMaxLength(100)
                .HasColumnName("event_type");
            entity.Property(e => e.Payload)
                .HasColumnType("jsonb")
                .HasColumnName("payload");
            entity.Property(e => e.Recipient)
                .HasMaxLength(255)
                .HasColumnName("recipient");
            entity.Property(e => e.SentAt).HasColumnName("sent_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Channel)
                .HasColumnName("channel");
            entity.Property(e => e.Status)
                .HasColumnName("status");

            entity.HasOne(d => d.User).WithMany(p => p.NotificationLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("notification_logs_user_id_fkey");
        });

        modelBuilder.Entity<OtpToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("otp_tokens_pkey");

            entity.ToTable("otp_tokens", "identity");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.AttemptCount)
                .HasDefaultValue((short)0)
                .HasColumnName("attempt_count");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.OtpHash).HasColumnName("otp_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .HasColumnName("phone");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VerifiedAt).HasColumnName("verified_at");
            entity.Property(e => e.Purpose)
                .HasColumnName("purpose");

            entity.HasOne(d => d.User).WithMany(p => p.OtpTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("otp_tokens_user_id_fkey");
        });

        modelBuilder.Entity<OwnerPayout>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("owner_payouts_pkey");

            entity.ToTable("owner_payouts", "payment");

            entity.HasIndex(e => new { e.OwnerId, e.PeriodFrom, e.PeriodTo }, "owner_payouts_owner_id_period_from_period_to_idx");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.BankAccount)
                .HasMaxLength(50)
                .HasColumnName("bank_account");
            entity.Property(e => e.GrossAmountVnd).HasColumnName("gross_amount_vnd");
            entity.Property(e => e.NetAmountVnd).HasColumnName("net_amount_vnd");
            entity.Property(e => e.OwnerId).HasColumnName("owner_id");
            entity.Property(e => e.PaidAt).HasColumnName("paid_at");
            entity.Property(e => e.PeriodFrom).HasColumnName("period_from");
            entity.Property(e => e.PeriodTo).HasColumnName("period_to");
            entity.Property(e => e.PlatformFeeVnd).HasColumnName("platform_fee_vnd");
            entity.Property(e => e.Status)
                .HasColumnName("status");

            entity.HasOne(d => d.Owner).WithMany(p => p.OwnerPayouts)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("owner_payouts_owner_id_fkey");
        });

        modelBuilder.Entity<Penalty>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("penalties_pkey");

            entity.ToTable("penalties", "ops");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.AmountVnd).HasColumnName("amount_vnd");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.ChargedTo).HasColumnName("charged_to");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EvidenceUrl).HasColumnName("evidence_url");
            entity.Property(e => e.Type)
                .HasColumnName("type");
            entity.Property(e => e.Status)
                .HasColumnName("status");

            entity.HasOne(d => d.Booking).WithMany(p => p.Penalties)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("penalties_booking_id_fkey");

            entity.HasOne(d => d.ChargedToNavigation).WithMany(p => p.Penalties)
                .HasForeignKey(d => d.ChargedTo)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("penalties_charged_to_fkey");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("permissions_pkey");

            entity.ToTable("permissions", "identity");

            entity.HasIndex(e => new { e.Resource, e.Action }, "permissions_resource_action_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Action)
                .HasMaxLength(50)
                .HasColumnName("action");
            entity.Property(e => e.Resource)
                .HasMaxLength(100)
                .HasColumnName("resource");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("promotions_pkey");

            entity.ToTable("promotions", "booking");

            entity.HasIndex(e => e.Code, "promotions_code_idx").HasFilter("(is_active = true)");

            entity.HasIndex(e => e.Code, "promotions_code_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.DiscountValue).HasColumnName("discount_value");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.MaxDiscountVnd).HasColumnName("max_discount_vnd");
            entity.Property(e => e.MinBookingVnd).HasColumnName("min_booking_vnd");
            entity.Property(e => e.UsageLimit).HasColumnName("usage_limit");
            entity.Property(e => e.UsedCount)
                .HasDefaultValue(0)
                .HasColumnName("used_count");
            entity.Property(e => e.ValidFrom).HasColumnName("valid_from");
            entity.Property(e => e.ValidTo).HasColumnName("valid_to");
            entity.Property(e => e.DiscountType)
                .HasColumnName("discount_type");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("refresh_tokens_pkey");

            entity.ToTable("refresh_tokens", "identity");

            entity.HasIndex(e => e.TokenHash, "refresh_tokens_token_hash_key").IsUnique();

            entity.HasIndex(e => e.UserId, "refresh_tokens_user_id_idx").HasFilter("(revoked_at IS NULL)");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.DeviceInfo)
                .HasColumnType("jsonb")
                .HasColumnName("device_info");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.IpAddress).HasColumnName("ip_address");
            entity.Property(e => e.RevokedAt).HasColumnName("revoked_at");
            entity.Property(e => e.TokenHash).HasColumnName("token_hash");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("refresh_tokens_user_id_fkey");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("reviews_pkey");

            entity.ToTable("reviews", "ops");

            entity.HasIndex(e => new { e.BookingId, e.ReviewerId }, "reviews_booking_id_reviewer_id_key").IsUnique();

            entity.HasIndex(e => e.CarId, "reviews_car_id_idx");

            entity.HasIndex(e => e.RevieweeId, "reviews_reviewee_id_idx");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CarId).HasColumnName("car_id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.RevieweeId).HasColumnName("reviewee_id");
            entity.Property(e => e.ReviewerId).HasColumnName("reviewer_id");

            entity.HasOne(d => d.Booking).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("reviews_booking_id_fkey");

            entity.HasOne(d => d.Car).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("reviews_car_id_fkey");

            entity.HasOne(d => d.Reviewee).WithMany(p => p.ReviewReviewees)
                .HasForeignKey(d => d.RevieweeId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("reviews_reviewee_id_fkey");

            entity.HasOne(d => d.Reviewer).WithMany(p => p.ReviewReviewers)
                .HasForeignKey(d => d.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("reviews_reviewer_id_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles", "identity");

            entity.HasIndex(e => e.Name, "roles_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");

            entity.HasMany(d => d.Permissions).WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RolePermission",
                    r => r.HasOne<Permission>().WithMany()
                        .HasForeignKey("PermissionId")
                        .HasConstraintName("role_permissions_permission_id_fkey"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("role_permissions_role_id_fkey"),
                    j =>
                    {
                        j.HasKey("RoleId", "PermissionId").HasName("role_permissions_pkey");
                        j.ToTable("role_permissions", "identity");
                        j.IndexerProperty<Guid>("RoleId").HasColumnName("role_id");
                        j.IndexerProperty<Guid>("PermissionId").HasColumnName("permission_id");
                    });
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("transactions_pkey");

            entity.ToTable("transactions", "payment");

            entity.HasIndex(e => e.BookingId, "transactions_booking_id_idx");

            entity.HasIndex(e => e.IdempotencyKey, "transactions_idempotency_key_key").IsUnique();

            entity.HasIndex(e => new { e.PayerId, e.CreatedAt }, "transactions_payer_id_created_at_idx").IsDescending(false, true);

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.AmountVnd).HasColumnName("amount_vnd");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.GatewayMeta)
                .HasColumnType("jsonb")
                .HasColumnName("gateway_meta");
            entity.Property(e => e.GatewayTxId)
                .HasMaxLength(100)
                .HasColumnName("gateway_tx_id");
            entity.Property(e => e.IdempotencyKey)
                .HasMaxLength(100)
                .HasColumnName("idempotency_key");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.PaidAt).HasColumnName("paid_at");
            entity.Property(e => e.PayerId).HasColumnName("payer_id");
            entity.Property(e => e.PayoutId).HasColumnName("payout_id");
            entity.Property(e => e.Direction)
                .HasColumnName("direction");
            entity.Property(e => e.Method)
                .HasColumnName("method");
            entity.Property(e => e.Status)
                .HasColumnName("status");

            entity.HasOne(d => d.Booking).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("transactions_booking_id_fkey");

            entity.HasOne(d => d.Payer).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.PayerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("transactions_payer_id_fkey");

            entity.HasOne(d => d.Payout).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.PayoutId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("transactions_payout_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users", "identity");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.HasIndex(e => e.Phone, "users_phone_idx");

            entity.HasIndex(e => e.Phone, "users_phone_key").IsUnique();

            entity.HasIndex(e => e.ReferralCode, "users_referral_code_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.AvatarUrl).HasColumnName("avatar_url");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .HasColumnName("phone");
            entity.Property(e => e.ReferralCode)
                .HasMaxLength(20)
                .HasColumnName("referral_code");
            entity.Property(e => e.ReferredById).HasColumnName("referred_by_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasColumnType("identity.user_status");

            entity.HasOne(d => d.ReferredBy).WithMany(p => p.InverseReferredBy)
                .HasForeignKey(d => d.ReferredById)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("users_referred_by_id_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("users_role_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
