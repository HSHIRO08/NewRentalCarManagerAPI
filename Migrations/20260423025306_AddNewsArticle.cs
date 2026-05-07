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
            migrationBuilder.Sql(@"
DO $$
BEGIN
    IF to_regclass('public.""NewsArticle""') IS NOT NULL AND to_regclass('public.news_articles') IS NULL THEN
        ALTER TABLE ""NewsArticle"" RENAME TO news_articles;
    END IF;
END $$;

DO $$
BEGIN
    IF to_regclass('public.news_articles') IS NOT NULL AND EXISTS (
        SELECT 1
        FROM information_schema.columns
        WHERE table_schema = 'public' AND table_name = 'news_articles' AND column_name = 'AuthorId'
    ) AND NOT EXISTS (
        SELECT 1
        FROM information_schema.columns
        WHERE table_schema = 'public' AND table_name = 'news_articles' AND column_name = 'author_id'
    ) THEN
        ALTER TABLE news_articles RENAME COLUMN ""AuthorId"" TO author_id;
    END IF;
END $$;

ALTER TABLE IF EXISTS identity.users
    ADD COLUMN IF NOT EXISTS status identity.user_status NOT NULL DEFAULT 'Pending'::identity.user_status;

ALTER TABLE IF EXISTS payment.transactions
    ADD COLUMN IF NOT EXISTS direction payment.payment_direction NOT NULL DEFAULT 'Charge'::payment.payment_direction;
ALTER TABLE IF EXISTS payment.transactions
    ADD COLUMN IF NOT EXISTS method payment.payment_method NOT NULL DEFAULT 'BankTransfer'::payment.payment_method;
ALTER TABLE IF EXISTS payment.transactions
    ADD COLUMN IF NOT EXISTS status payment.payment_status NOT NULL DEFAULT 'Pending'::payment.payment_status;

ALTER TABLE IF EXISTS booking.promotions
    ADD COLUMN IF NOT EXISTS discount_type booking.promotion_type NOT NULL DEFAULT 'Percentage'::booking.promotion_type;

ALTER TABLE IF EXISTS ops.penalties
    ADD COLUMN IF NOT EXISTS status ops.penalty_status NOT NULL DEFAULT 'Pending'::ops.penalty_status;
ALTER TABLE IF EXISTS ops.penalties
    ADD COLUMN IF NOT EXISTS type ops.penalty_type NOT NULL DEFAULT 'TrafficFine'::ops.penalty_type;

ALTER TABLE IF EXISTS payment.owner_payouts
    ADD COLUMN IF NOT EXISTS status payment.payment_status NOT NULL DEFAULT 'Pending'::payment.payment_status;

ALTER TABLE IF EXISTS identity.otp_tokens
    ADD COLUMN IF NOT EXISTS purpose identity.otp_purpose NOT NULL DEFAULT 'Login'::identity.otp_purpose;

ALTER TABLE IF EXISTS notification.notification_logs
    ADD COLUMN IF NOT EXISTS channel notification.notif_channel NOT NULL DEFAULT 'Push'::notification.notif_channel;
ALTER TABLE IF EXISTS notification.notification_logs
    ADD COLUMN IF NOT EXISTS status notification.notif_status NOT NULL DEFAULT 'Pending'::notification.notif_status;

ALTER TABLE IF EXISTS identity.external_logins
    ADD COLUMN IF NOT EXISTS provider_name identity.provider_name NOT NULL DEFAULT 'Google'::identity.provider_name;

ALTER TABLE IF EXISTS ops.damage_reports
    ADD COLUMN IF NOT EXISTS severity ops.damage_severity NOT NULL DEFAULT 'Minor'::ops.damage_severity;
ALTER TABLE IF EXISTS ops.damage_reports
    ADD COLUMN IF NOT EXISTS status ops.damage_status NOT NULL DEFAULT 'Reported'::ops.damage_status;

ALTER TABLE IF EXISTS fleet.cars
    ADD COLUMN IF NOT EXISTS fuel_type fleet.fuel_type NOT NULL DEFAULT 'Gasoline'::fleet.fuel_type;
ALTER TABLE IF EXISTS fleet.cars
    ADD COLUMN IF NOT EXISTS status fleet.car_status NOT NULL DEFAULT 'Available'::fleet.car_status;
ALTER TABLE IF EXISTS fleet.cars
    ADD COLUMN IF NOT EXISTS transmission fleet.transmission_type NOT NULL DEFAULT 'Automatic'::fleet.transmission_type;

ALTER TABLE IF EXISTS fleet.car_pricing
    ADD COLUMN IF NOT EXISTS rental_type fleet.rental_type NOT NULL DEFAULT 'Hourly'::fleet.rental_type;

ALTER TABLE IF EXISTS booking.bookings
    ADD COLUMN IF NOT EXISTS saga_status booking.saga_status NOT NULL DEFAULT 'Started'::booking.saga_status;
ALTER TABLE IF EXISTS booking.bookings
    ADD COLUMN IF NOT EXISTS status booking.booking_status NOT NULL DEFAULT 'Pending'::booking.booking_status;

ALTER TABLE IF EXISTS news_articles
    ADD COLUMN IF NOT EXISTS id uuid DEFAULT gen_random_uuid();
ALTER TABLE IF EXISTS news_articles
    ADD COLUMN IF NOT EXISTS category text;
ALTER TABLE IF EXISTS news_articles
    ADD COLUMN IF NOT EXISTS content text NOT NULL DEFAULT '';
ALTER TABLE IF EXISTS news_articles
    ADD COLUMN IF NOT EXISTS created_at timestamp with time zone NOT NULL DEFAULT now();
ALTER TABLE IF EXISTS news_articles
    ADD COLUMN IF NOT EXISTS image_url text;
ALTER TABLE IF EXISTS news_articles
    ADD COLUMN IF NOT EXISTS reject_reason text;
ALTER TABLE IF EXISTS news_articles
    ADD COLUMN IF NOT EXISTS status text NOT NULL DEFAULT 'pending';
ALTER TABLE IF EXISTS news_articles
    ADD COLUMN IF NOT EXISTS summary text NOT NULL DEFAULT '';
ALTER TABLE IF EXISTS news_articles
    ADD COLUMN IF NOT EXISTS title text NOT NULL DEFAULT '';
ALTER TABLE IF EXISTS news_articles
    ADD COLUMN IF NOT EXISTS updated_at timestamp with time zone NOT NULL DEFAULT now();

DO $$
BEGIN
    IF to_regclass('public.news_articles') IS NOT NULL AND NOT EXISTS (
        SELECT 1 FROM pg_constraint WHERE conname = 'news_articles_pkey'
    ) THEN
        ALTER TABLE news_articles ADD CONSTRAINT news_articles_pkey PRIMARY KEY (id);
    END IF;
END $$;

DO $$
BEGIN
    IF to_regclass('public.news_articles') IS NOT NULL THEN
        CREATE INDEX IF NOT EXISTS news_articles_author_id_idx ON news_articles(author_id);
    END IF;
END $$;
");
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
