using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NewRentalCarManagerAPI.Enums;

#nullable disable

namespace NewRentalCarManagerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddNewsArticle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "NewsArticle",
                newName: "news_articles");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "news_articles",
                newName: "author_id");

            migrationBuilder.AddColumn<UserStatus>(
                name: "status",
                schema: "identity",
                table: "users",
                type: "identity.user_status",
                nullable: false,
                defaultValue: UserStatus.Pending);

            migrationBuilder.AddColumn<PaymentDirection>(
                name: "direction",
                schema: "payment",
                table: "transactions",
                type: "payment.payment_direction",
                nullable: false,
                defaultValue: PaymentDirection.Charge);

            migrationBuilder.AddColumn<PaymentMethod>(
                name: "method",
                schema: "payment",
                table: "transactions",
                type: "payment.payment_method",
                nullable: false,
                defaultValue: PaymentMethod.BankTransfer);

            migrationBuilder.AddColumn<PaymentStatus>(
                name: "status",
                schema: "payment",
                table: "transactions",
                type: "payment.payment_status",
                nullable: false,
                defaultValue: PaymentStatus.Pending);

            migrationBuilder.AddColumn<PromotionType>(
                name: "discount_type",
                schema: "booking",
                table: "promotions",
                type: "booking.promotion_type",
                nullable: false,
                defaultValue: PromotionType.Percentage);

            migrationBuilder.AddColumn<PenaltyStatus>(
                name: "status",
                schema: "ops",
                table: "penalties",
                type: "ops.penalty_status",
                nullable: false,
                defaultValue: PenaltyStatus.Pending);

            migrationBuilder.AddColumn<PenaltyType>(
                name: "type",
                schema: "ops",
                table: "penalties",
                type: "ops.penalty_type",
                nullable: false,
                defaultValue: PenaltyType.TrafficFine);

            migrationBuilder.AddColumn<PaymentStatus>(
                name: "status",
                schema: "payment",
                table: "owner_payouts",
                type: "payment.payment_status",
                nullable: false,
                defaultValue: PaymentStatus.Pending);

            migrationBuilder.AddColumn<OtpPurpose>(
                name: "purpose",
                schema: "identity",
                table: "otp_tokens",
                type: "identity.otp_purpose",
                nullable: false,
                defaultValue: OtpPurpose.Login);

            migrationBuilder.AddColumn<NotifChannel>(
                name: "channel",
                schema: "notification",
                table: "notification_logs",
                type: "notification.notif_channel",
                nullable: false,
                defaultValue: NotifChannel.Push);

            migrationBuilder.AddColumn<NotifStatus>(
                name: "status",
                schema: "notification",
                table: "notification_logs",
                type: "notification.notif_status",
                nullable: false,
                defaultValue: NotifStatus.Pending);

            migrationBuilder.AddColumn<ProviderName>(
                name: "provider_name",
                schema: "identity",
                table: "external_logins",
                type: "identity.provider_name",
                nullable: false,
                defaultValue: ProviderName.Google);

            migrationBuilder.AddColumn<DamageSeverity>(
                name: "severity",
                schema: "ops",
                table: "damage_reports",
                type: "ops.damage_severity",
                nullable: false,
                defaultValue: DamageSeverity.Minor);

            migrationBuilder.AddColumn<DamageStatus>(
                name: "status",
                schema: "ops",
                table: "damage_reports",
                type: "ops.damage_status",
                nullable: false,
                defaultValue: DamageStatus.Reported);

            migrationBuilder.AddColumn<FuelType>(
                name: "fuel_type",
                schema: "fleet",
                table: "cars",
                type: "fleet.fuel_type",
                nullable: false,
                defaultValue: FuelType.Gasoline);

            migrationBuilder.AddColumn<CarStatus>(
                name: "status",
                schema: "fleet",
                table: "cars",
                type: "fleet.car_status",
                nullable: false,
                defaultValue: CarStatus.Available);

            migrationBuilder.AddColumn<TransmissionType>(
                name: "transmission",
                schema: "fleet",
                table: "cars",
                type: "fleet.transmission_type",
                nullable: false,
                defaultValue: TransmissionType.Automatic);

            migrationBuilder.AddColumn<RentalType>(
                name: "rental_type",
                schema: "fleet",
                table: "car_pricing",
                type: "fleet.rental_type",
                nullable: false,
                defaultValue: RentalType.Hourly);

            migrationBuilder.AddColumn<SagaStatus>(
                name: "saga_status",
                schema: "booking",
                table: "bookings",
                type: "booking.saga_status",
                nullable: false,
                defaultValue: SagaStatus.Started);

            migrationBuilder.AddColumn<BookingStatus>(
                name: "status",
                schema: "booking",
                table: "bookings",
                type: "booking.booking_status",
                nullable: false,
                defaultValue: BookingStatus.Pending);

            migrationBuilder.AlterColumn<Guid>(
                name: "author_id",
                table: "news_articles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                table: "news_articles",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddColumn<string>(
                name: "category",
                table: "news_articles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "content",
                table: "news_articles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "news_articles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AddColumn<string>(
                name: "image_url",
                table: "news_articles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reject_reason",
                table: "news_articles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "news_articles",
                type: "text",
                nullable: false,
                defaultValue: "pending");

            migrationBuilder.AddColumn<string>(
                name: "summary",
                table: "news_articles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "title",
                table: "news_articles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "news_articles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AddPrimaryKey(
                name: "news_articles_pkey",
                table: "news_articles",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "news_articles_author_id_idx",
                table: "news_articles",
                column: "author_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "news_articles_pkey",
                table: "news_articles");

            migrationBuilder.DropIndex(
                name: "news_articles_author_id_idx",
                table: "news_articles");

            migrationBuilder.DropColumn(
                name: "status",
                schema: "identity",
                table: "users");

            migrationBuilder.DropColumn(
                name: "direction",
                schema: "payment",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "method",
                schema: "payment",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "status",
                schema: "payment",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "discount_type",
                schema: "booking",
                table: "promotions");

            migrationBuilder.DropColumn(
                name: "status",
                schema: "ops",
                table: "penalties");

            migrationBuilder.DropColumn(
                name: "type",
                schema: "ops",
                table: "penalties");

            migrationBuilder.DropColumn(
                name: "status",
                schema: "payment",
                table: "owner_payouts");

            migrationBuilder.DropColumn(
                name: "purpose",
                schema: "identity",
                table: "otp_tokens");

            migrationBuilder.DropColumn(
                name: "channel",
                schema: "notification",
                table: "notification_logs");

            migrationBuilder.DropColumn(
                name: "status",
                schema: "notification",
                table: "notification_logs");

            migrationBuilder.DropColumn(
                name: "provider_name",
                schema: "identity",
                table: "external_logins");

            migrationBuilder.DropColumn(
                name: "severity",
                schema: "ops",
                table: "damage_reports");

            migrationBuilder.DropColumn(
                name: "status",
                schema: "ops",
                table: "damage_reports");

            migrationBuilder.DropColumn(
                name: "fuel_type",
                schema: "fleet",
                table: "cars");

            migrationBuilder.DropColumn(
                name: "status",
                schema: "fleet",
                table: "cars");

            migrationBuilder.DropColumn(
                name: "transmission",
                schema: "fleet",
                table: "cars");

            migrationBuilder.DropColumn(
                name: "rental_type",
                schema: "fleet",
                table: "car_pricing");

            migrationBuilder.DropColumn(
                name: "saga_status",
                schema: "booking",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "status",
                schema: "booking",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "id",
                table: "news_articles");

            migrationBuilder.DropColumn(
                name: "category",
                table: "news_articles");

            migrationBuilder.DropColumn(
                name: "content",
                table: "news_articles");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "news_articles");

            migrationBuilder.DropColumn(
                name: "image_url",
                table: "news_articles");

            migrationBuilder.DropColumn(
                name: "reject_reason",
                table: "news_articles");

            migrationBuilder.DropColumn(
                name: "status",
                table: "news_articles");

            migrationBuilder.DropColumn(
                name: "summary",
                table: "news_articles");

            migrationBuilder.DropColumn(
                name: "title",
                table: "news_articles");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "news_articles");

            migrationBuilder.RenameTable(
                name: "news_articles",
                newName: "NewsArticle");

            migrationBuilder.RenameColumn(
                name: "author_id",
                table: "NewsArticle",
                newName: "AuthorId");

            migrationBuilder.AlterColumn<Guid>(
                name: "AuthorId",
                table: "NewsArticle",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }
    }
}
