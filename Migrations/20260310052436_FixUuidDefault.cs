using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewRentalCarManagerAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixUuidDefault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "identity",
                table: "users",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v7()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "payment",
                table: "transactions",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v7()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "identity",
                table: "roles",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v7()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "ops",
                table: "reviews",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v7()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "identity",
                table: "refresh_tokens",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v7()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "booking",
                table: "promotions",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v7()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "identity",
                table: "permissions",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v7()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "ops",
                table: "penalties",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v7()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "payment",
                table: "owner_payouts",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v7()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "identity",
                table: "otp_tokens",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v7()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "notification",
                table: "notification_logs",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v7()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "fleet",
                table: "locations",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v7()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "identity",
                table: "external_logins",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v7()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "ops",
                table: "damage_reports",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v7()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "fleet",
                table: "cars",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v7()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "fleet",
                table: "car_pricing",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v7()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "fleet",
                table: "car_models",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v7()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "fleet",
                table: "car_brands",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v7()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "fleet",
                table: "car_availability_blocks",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v7()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "booking",
                table: "bookings",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v7()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "identity",
                table: "api_keys",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v7()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "identity",
                table: "users",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v7()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "payment",
                table: "transactions",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v7()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "identity",
                table: "roles",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v7()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "ops",
                table: "reviews",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v7()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "identity",
                table: "refresh_tokens",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v7()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "booking",
                table: "promotions",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v7()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "identity",
                table: "permissions",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v7()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "ops",
                table: "penalties",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v7()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "payment",
                table: "owner_payouts",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v7()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "identity",
                table: "otp_tokens",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v7()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "notification",
                table: "notification_logs",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v7()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "fleet",
                table: "locations",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v7()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "identity",
                table: "external_logins",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v7()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "ops",
                table: "damage_reports",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v7()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "fleet",
                table: "cars",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v7()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "fleet",
                table: "car_pricing",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v7()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "fleet",
                table: "car_models",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v7()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "fleet",
                table: "car_brands",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v7()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "fleet",
                table: "car_availability_blocks",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v7()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "booking",
                table: "bookings",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v7()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "identity",
                table: "api_keys",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v7()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");
        }
    }
}
