using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewRentalCarManagerAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "identity");

            migrationBuilder.EnsureSchema(
                name: "booking");

            migrationBuilder.EnsureSchema(
                name: "fleet");

            migrationBuilder.EnsureSchema(
                name: "ops");

            migrationBuilder.EnsureSchema(
                name: "notification");

            migrationBuilder.EnsureSchema(
                name: "payment");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:booking.booking_status", "pending,confirmed,active,completed,cancelled,disputed")
                .Annotation("Npgsql:Enum:booking.promotion_type", "percentage,fixed_amount")
                .Annotation("Npgsql:Enum:booking.saga_status", "started,succeeded,failed,compensating")
                .Annotation("Npgsql:Enum:fleet.car_status", "available,rented,maintenance,inactive")
                .Annotation("Npgsql:Enum:fleet.fuel_type", "gasoline,diesel,electric,hybrid")
                .Annotation("Npgsql:Enum:fleet.rental_type", "hourly,daily")
                .Annotation("Npgsql:Enum:fleet.transmission_type", "automatic,manual")
                .Annotation("Npgsql:Enum:identity.otp_purpose", "login,register,reset_password,verify_phone")
                .Annotation("Npgsql:Enum:identity.provider_name", "google,facebook,zalo,apple")
                .Annotation("Npgsql:Enum:identity.user_status", "pending,active,suspended,banned")
                .Annotation("Npgsql:Enum:notification.notif_channel", "push,sms,email,in_app")
                .Annotation("Npgsql:Enum:notification.notif_status", "pending,sent,failed,skipped")
                .Annotation("Npgsql:Enum:ops.damage_severity", "minor,moderate,severe")
                .Annotation("Npgsql:Enum:ops.damage_status", "reported,assessed,resolved,disputed")
                .Annotation("Npgsql:Enum:ops.penalty_status", "pending,charged,disputed,waived")
                .Annotation("Npgsql:Enum:ops.penalty_type", "traffic_fine,toll_violation,late_return,fuel_shortage,cleaning_fee")
                .Annotation("Npgsql:Enum:payment.payment_direction", "charge,refund,payout")
                .Annotation("Npgsql:Enum:payment.payment_method", "bank_transfer,momo,vnpay,zalopay,credit_card")
                .Annotation("Npgsql:Enum:payment.payment_status", "pending,success,failed,refunded")
                .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,");

            migrationBuilder.CreateTable(
                name: "car_brands",
                schema: "fleet",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    logo_url = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("car_brands_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "locations",
                schema: "fleet",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    district = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ward = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    address = table.Column<string>(type: "text", nullable: true),
                    latitude = table.Column<decimal>(type: "numeric(10,7)", precision: 10, scale: 7, nullable: true),
                    longitude = table.Column<decimal>(type: "numeric(10,7)", precision: 10, scale: 7, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("locations_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    resource = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("permissions_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "promotions",
                schema: "booking",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    discount_value = table.Column<int>(type: "integer", nullable: false),
                    max_discount_vnd = table.Column<int>(type: "integer", nullable: true),
                    min_booking_vnd = table.Column<int>(type: "integer", nullable: true),
                    usage_limit = table.Column<int>(type: "integer", nullable: true),
                    used_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    valid_from = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    valid_to = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("promotions_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("roles_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "car_models",
                schema: "fleet",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    seat_count = table.Column<short>(type: "smallint", nullable: false),
                    car_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("car_models_pkey", x => x.id);
                    table.ForeignKey(
                        name: "car_models_brand_id_fkey",
                        column: x => x.brand_id,
                        principalSchema: "fleet",
                        principalTable: "car_brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "role_permissions",
                schema: "identity",
                columns: table => new
                {
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    permission_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("role_permissions_pkey", x => new { x.role_id, x.permission_id });
                    table.ForeignKey(
                        name: "role_permissions_permission_id_fkey",
                        column: x => x.permission_id,
                        principalSchema: "identity",
                        principalTable: "permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "role_permissions_role_id_fkey",
                        column: x => x.role_id,
                        principalSchema: "identity",
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    phone = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    full_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    avatar_url = table.Column<string>(type: "text", nullable: true),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    referral_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    referred_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("users_pkey", x => x.id);
                    table.ForeignKey(
                        name: "users_referred_by_id_fkey",
                        column: x => x.referred_by_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "users_role_id_fkey",
                        column: x => x.role_id,
                        principalSchema: "identity",
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "api_keys",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    key_hash = table.Column<string>(type: "text", nullable: false),
                    key_prefix = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    scopes = table.Column<List<string>>(type: "text[]", nullable: false, defaultValueSql: "'{}'::text[]"),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_used_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    revoked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("api_keys_pkey", x => x.id);
                    table.ForeignKey(
                        name: "api_keys_user_id_fkey",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cars",
                schema: "fleet",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    model_id = table.Column<Guid>(type: "uuid", nullable: false),
                    location_id = table.Column<Guid>(type: "uuid", nullable: false),
                    license_plate = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    manufacture_year = table.Column<short>(type: "smallint", nullable: false),
                    color = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    mileage_km = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    description = table.Column<string>(type: "text", nullable: true),
                    image_urls = table.Column<List<string>>(type: "text[]", nullable: false, defaultValueSql: "'{}'::text[]"),
                    features = table.Column<List<string>>(type: "text[]", nullable: false, defaultValueSql: "'{}'::text[]"),
                    has_iot_device = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    iot_device_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("cars_pkey", x => x.id);
                    table.ForeignKey(
                        name: "cars_location_id_fkey",
                        column: x => x.location_id,
                        principalSchema: "fleet",
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "cars_model_id_fkey",
                        column: x => x.model_id,
                        principalSchema: "fleet",
                        principalTable: "car_models",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "cars_owner_id_fkey",
                        column: x => x.owner_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "external_logins",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    provider_uid = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    access_token = table.Column<string>(type: "text", nullable: true),
                    refresh_token = table.Column<string>(type: "text", nullable: true),
                    token_expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    profile_data = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("external_logins_pkey", x => x.id);
                    table.ForeignKey(
                        name: "external_logins_user_id_fkey",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notification_logs",
                schema: "notification",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    recipient = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    payload = table.Column<string>(type: "jsonb", nullable: false),
                    error_msg = table.Column<string>(type: "text", nullable: true),
                    sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("notification_logs_pkey", x => x.id);
                    table.ForeignKey(
                        name: "notification_logs_user_id_fkey",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "otp_tokens",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    phone = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    otp_hash = table.Column<string>(type: "text", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    verified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    attempt_count = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("otp_tokens_pkey", x => x.id);
                    table.ForeignKey(
                        name: "otp_tokens_user_id_fkey",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "owner_payouts",
                schema: "payment",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    period_from = table.Column<DateOnly>(type: "date", nullable: false),
                    period_to = table.Column<DateOnly>(type: "date", nullable: false),
                    gross_amount_vnd = table.Column<int>(type: "integer", nullable: false),
                    platform_fee_vnd = table.Column<int>(type: "integer", nullable: false),
                    net_amount_vnd = table.Column<int>(type: "integer", nullable: false),
                    paid_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    bank_account = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("owner_payouts_pkey", x => x.id);
                    table.ForeignKey(
                        name: "owner_payouts_owner_id_fkey",
                        column: x => x.owner_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_hash = table.Column<string>(type: "text", nullable: false),
                    device_info = table.Column<string>(type: "jsonb", nullable: true),
                    ip_address = table.Column<IPAddress>(type: "inet", nullable: true),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revoked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("refresh_tokens_pkey", x => x.id);
                    table.ForeignKey(
                        name: "refresh_tokens_user_id_fkey",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "car_availability_blocks",
                schema: "fleet",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    car_id = table.Column<Guid>(type: "uuid", nullable: false),
                    blocked_from = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    blocked_to = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    source = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'owner'::character varying"),
                    booking_id = table.Column<Guid>(type: "uuid", nullable: true),
                    reason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("car_availability_blocks_pkey", x => x.id);
                    table.ForeignKey(
                        name: "car_availability_blocks_car_id_fkey",
                        column: x => x.car_id,
                        principalSchema: "fleet",
                        principalTable: "cars",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "car_pricing",
                schema: "fleet",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    car_id = table.Column<Guid>(type: "uuid", nullable: false),
                    duration_hours = table.Column<short>(type: "smallint", nullable: true),
                    price_vnd = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("car_pricing_pkey", x => x.id);
                    table.ForeignKey(
                        name: "car_pricing_car_id_fkey",
                        column: x => x.car_id,
                        principalSchema: "fleet",
                        principalTable: "cars",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "bookings",
                schema: "booking",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    renter_id = table.Column<Guid>(type: "uuid", nullable: false),
                    car_id = table.Column<Guid>(type: "uuid", nullable: false),
                    pricing_id = table.Column<Guid>(type: "uuid", nullable: false),
                    promotion_id = table.Column<Guid>(type: "uuid", nullable: true),
                    pickup_location_id = table.Column<Guid>(type: "uuid", nullable: false),
                    return_location_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rental_start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    rental_end = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    actual_return_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    base_price_vnd = table.Column<int>(type: "integer", nullable: false),
                    discount_vnd = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    surcharge_vnd = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    delivery_fee_vnd = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    total_price_vnd = table.Column<int>(type: "integer", nullable: false),
                    vat_vnd = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    deposit_vnd = table.Column<int>(type: "integer", nullable: false),
                    cancel_reason = table.Column<string>(type: "text", nullable: true),
                    note = table.Column<string>(type: "text", nullable: true),
                    saga_step = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    saga_context = table.Column<string>(type: "jsonb", nullable: true, defaultValueSql: "'{}'::jsonb"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("bookings_pkey", x => x.id);
                    table.ForeignKey(
                        name: "bookings_car_id_fkey",
                        column: x => x.car_id,
                        principalSchema: "fleet",
                        principalTable: "cars",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "bookings_pickup_location_id_fkey",
                        column: x => x.pickup_location_id,
                        principalSchema: "fleet",
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "bookings_pricing_id_fkey",
                        column: x => x.pricing_id,
                        principalSchema: "fleet",
                        principalTable: "car_pricing",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "bookings_promotion_id_fkey",
                        column: x => x.promotion_id,
                        principalSchema: "booking",
                        principalTable: "promotions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "bookings_renter_id_fkey",
                        column: x => x.renter_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "bookings_return_location_id_fkey",
                        column: x => x.return_location_id,
                        principalSchema: "fleet",
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "damage_reports",
                schema: "ops",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    booking_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reported_by = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    image_urls = table.Column<List<string>>(type: "text[]", nullable: false, defaultValueSql: "'{}'::text[]"),
                    repair_cost_vnd = table.Column<int>(type: "integer", nullable: true),
                    resolved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("damage_reports_pkey", x => x.id);
                    table.ForeignKey(
                        name: "damage_reports_booking_id_fkey",
                        column: x => x.booking_id,
                        principalSchema: "booking",
                        principalTable: "bookings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "damage_reports_reported_by_fkey",
                        column: x => x.reported_by,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "penalties",
                schema: "ops",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    booking_id = table.Column<Guid>(type: "uuid", nullable: false),
                    charged_to = table.Column<Guid>(type: "uuid", nullable: false),
                    amount_vnd = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    evidence_url = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("penalties_pkey", x => x.id);
                    table.ForeignKey(
                        name: "penalties_booking_id_fkey",
                        column: x => x.booking_id,
                        principalSchema: "booking",
                        principalTable: "bookings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "penalties_charged_to_fkey",
                        column: x => x.charged_to,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "reviews",
                schema: "ops",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    booking_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reviewer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reviewee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    car_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rating = table.Column<short>(type: "smallint", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("reviews_pkey", x => x.id);
                    table.ForeignKey(
                        name: "reviews_booking_id_fkey",
                        column: x => x.booking_id,
                        principalSchema: "booking",
                        principalTable: "bookings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "reviews_car_id_fkey",
                        column: x => x.car_id,
                        principalSchema: "fleet",
                        principalTable: "cars",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "reviews_reviewee_id_fkey",
                        column: x => x.reviewee_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "reviews_reviewer_id_fkey",
                        column: x => x.reviewer_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                schema: "payment",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    booking_id = table.Column<Guid>(type: "uuid", nullable: false),
                    payer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    payout_id = table.Column<Guid>(type: "uuid", nullable: true),
                    amount_vnd = table.Column<int>(type: "integer", nullable: false),
                    gateway_tx_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    gateway_meta = table.Column<string>(type: "jsonb", nullable: true),
                    idempotency_key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    note = table.Column<string>(type: "text", nullable: true),
                    paid_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("transactions_pkey", x => x.id);
                    table.ForeignKey(
                        name: "transactions_booking_id_fkey",
                        column: x => x.booking_id,
                        principalSchema: "booking",
                        principalTable: "bookings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "transactions_payer_id_fkey",
                        column: x => x.payer_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "transactions_payout_id_fkey",
                        column: x => x.payout_id,
                        principalSchema: "payment",
                        principalTable: "owner_payouts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "api_keys_key_hash_key",
                schema: "identity",
                table: "api_keys",
                column: "key_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "api_keys_user_id_idx",
                schema: "identity",
                table: "api_keys",
                column: "user_id",
                filter: "(revoked_at IS NULL)");

            migrationBuilder.CreateIndex(
                name: "bookings_car_id_rental_start_rental_end_idx",
                schema: "booking",
                table: "bookings",
                columns: new[] { "car_id", "rental_start", "rental_end" });

            migrationBuilder.CreateIndex(
                name: "IX_bookings_pickup_location_id",
                schema: "booking",
                table: "bookings",
                column: "pickup_location_id");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_pricing_id",
                schema: "booking",
                table: "bookings",
                column: "pricing_id");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_promotion_id",
                schema: "booking",
                table: "bookings",
                column: "promotion_id");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_renter_id",
                schema: "booking",
                table: "bookings",
                column: "renter_id");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_return_location_id",
                schema: "booking",
                table: "bookings",
                column: "return_location_id");

            migrationBuilder.CreateIndex(
                name: "car_availability_blocks_car_id_blocked_from_blocked_to_idx",
                schema: "fleet",
                table: "car_availability_blocks",
                columns: new[] { "car_id", "blocked_from", "blocked_to" });

            migrationBuilder.CreateIndex(
                name: "car_brands_name_key",
                schema: "fleet",
                table: "car_brands",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "car_models_brand_id_name_key",
                schema: "fleet",
                table: "car_models",
                columns: new[] { "brand_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "car_pricing_car_id_idx",
                schema: "fleet",
                table: "car_pricing",
                column: "car_id",
                filter: "(is_active = true)");

            migrationBuilder.CreateIndex(
                name: "cars_features_idx",
                schema: "fleet",
                table: "cars",
                column: "features")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "cars_license_plate_key",
                schema: "fleet",
                table: "cars",
                column: "license_plate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cars_location_id",
                schema: "fleet",
                table: "cars",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_cars_model_id",
                schema: "fleet",
                table: "cars",
                column: "model_id");

            migrationBuilder.CreateIndex(
                name: "IX_cars_owner_id",
                schema: "fleet",
                table: "cars",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "damage_reports_booking_id_idx",
                schema: "ops",
                table: "damage_reports",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "IX_damage_reports_reported_by",
                schema: "ops",
                table: "damage_reports",
                column: "reported_by");

            migrationBuilder.CreateIndex(
                name: "IX_external_logins_user_id",
                schema: "identity",
                table: "external_logins",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "notification_logs_user_id_created_at_idx",
                schema: "notification",
                table: "notification_logs",
                columns: new[] { "user_id", "created_at" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_otp_tokens_user_id",
                schema: "identity",
                table: "otp_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "owner_payouts_owner_id_period_from_period_to_idx",
                schema: "payment",
                table: "owner_payouts",
                columns: new[] { "owner_id", "period_from", "period_to" });

            migrationBuilder.CreateIndex(
                name: "IX_penalties_booking_id",
                schema: "ops",
                table: "penalties",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "IX_penalties_charged_to",
                schema: "ops",
                table: "penalties",
                column: "charged_to");

            migrationBuilder.CreateIndex(
                name: "permissions_resource_action_key",
                schema: "identity",
                table: "permissions",
                columns: new[] { "resource", "action" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "promotions_code_idx",
                schema: "booking",
                table: "promotions",
                column: "code",
                filter: "(is_active = true)");

            migrationBuilder.CreateIndex(
                name: "promotions_code_key",
                schema: "booking",
                table: "promotions",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "refresh_tokens_token_hash_key",
                schema: "identity",
                table: "refresh_tokens",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "refresh_tokens_user_id_idx",
                schema: "identity",
                table: "refresh_tokens",
                column: "user_id",
                filter: "(revoked_at IS NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_reviews_reviewer_id",
                schema: "ops",
                table: "reviews",
                column: "reviewer_id");

            migrationBuilder.CreateIndex(
                name: "reviews_booking_id_reviewer_id_key",
                schema: "ops",
                table: "reviews",
                columns: new[] { "booking_id", "reviewer_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "reviews_car_id_idx",
                schema: "ops",
                table: "reviews",
                column: "car_id");

            migrationBuilder.CreateIndex(
                name: "reviews_reviewee_id_idx",
                schema: "ops",
                table: "reviews",
                column: "reviewee_id");

            migrationBuilder.CreateIndex(
                name: "IX_role_permissions_permission_id",
                schema: "identity",
                table: "role_permissions",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "roles_name_key",
                schema: "identity",
                table: "roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_transactions_payout_id",
                schema: "payment",
                table: "transactions",
                column: "payout_id");

            migrationBuilder.CreateIndex(
                name: "transactions_booking_id_idx",
                schema: "payment",
                table: "transactions",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "transactions_idempotency_key_key",
                schema: "payment",
                table: "transactions",
                column: "idempotency_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "transactions_payer_id_created_at_idx",
                schema: "payment",
                table: "transactions",
                columns: new[] { "payer_id", "created_at" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_users_referred_by_id",
                schema: "identity",
                table: "users",
                column: "referred_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_role_id",
                schema: "identity",
                table: "users",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "users_email_key",
                schema: "identity",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "users_phone_idx",
                schema: "identity",
                table: "users",
                column: "phone");

            migrationBuilder.CreateIndex(
                name: "users_phone_key",
                schema: "identity",
                table: "users",
                column: "phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "users_referral_code_key",
                schema: "identity",
                table: "users",
                column: "referral_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "api_keys",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "car_availability_blocks",
                schema: "fleet");

            migrationBuilder.DropTable(
                name: "damage_reports",
                schema: "ops");

            migrationBuilder.DropTable(
                name: "external_logins",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "notification_logs",
                schema: "notification");

            migrationBuilder.DropTable(
                name: "otp_tokens",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "penalties",
                schema: "ops");

            migrationBuilder.DropTable(
                name: "refresh_tokens",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "reviews",
                schema: "ops");

            migrationBuilder.DropTable(
                name: "role_permissions",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "transactions",
                schema: "payment");

            migrationBuilder.DropTable(
                name: "permissions",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "bookings",
                schema: "booking");

            migrationBuilder.DropTable(
                name: "owner_payouts",
                schema: "payment");

            migrationBuilder.DropTable(
                name: "car_pricing",
                schema: "fleet");

            migrationBuilder.DropTable(
                name: "promotions",
                schema: "booking");

            migrationBuilder.DropTable(
                name: "cars",
                schema: "fleet");

            migrationBuilder.DropTable(
                name: "locations",
                schema: "fleet");

            migrationBuilder.DropTable(
                name: "car_models",
                schema: "fleet");

            migrationBuilder.DropTable(
                name: "users",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "car_brands",
                schema: "fleet");

            migrationBuilder.DropTable(
                name: "roles",
                schema: "identity");
        }
    }
}
