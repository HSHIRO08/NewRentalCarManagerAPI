using Microsoft.EntityFrameworkCore.Migrations;
using NewRentalCarManagerAPI.Enums;

#nullable disable

namespace NewRentalCarManagerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddKycToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:booking.booking_status", "pending,confirmed,active,completed,cancelled,disputed")
                .Annotation("Npgsql:Enum:booking.promotion_type", "percentage,fixed_amount")
                .Annotation("Npgsql:Enum:booking.saga_status", "started,succeeded,failed,compensating")
                .Annotation("Npgsql:Enum:fleet.car_status", "available,rented,maintenance,inactive")
                .Annotation("Npgsql:Enum:fleet.fuel_type", "gasoline,diesel,electric,hybrid")
                .Annotation("Npgsql:Enum:fleet.rental_type", "hourly,daily")
                .Annotation("Npgsql:Enum:fleet.transmission_type", "automatic,manual")
                .Annotation("Npgsql:Enum:identity.kyc_status", "none,pending,approved,rejected")
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
                .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,")
                .OldAnnotation("Npgsql:Enum:booking.booking_status", "pending,confirmed,active,completed,cancelled,disputed")
                .OldAnnotation("Npgsql:Enum:booking.promotion_type", "percentage,fixed_amount")
                .OldAnnotation("Npgsql:Enum:booking.saga_status", "started,succeeded,failed,compensating")
                .OldAnnotation("Npgsql:Enum:fleet.car_status", "available,rented,maintenance,inactive")
                .OldAnnotation("Npgsql:Enum:fleet.fuel_type", "gasoline,diesel,electric,hybrid")
                .OldAnnotation("Npgsql:Enum:fleet.rental_type", "hourly,daily")
                .OldAnnotation("Npgsql:Enum:fleet.transmission_type", "automatic,manual")
                .OldAnnotation("Npgsql:Enum:identity.otp_purpose", "login,register,reset_password,verify_phone")
                .OldAnnotation("Npgsql:Enum:identity.provider_name", "google,facebook,zalo,apple")
                .OldAnnotation("Npgsql:Enum:identity.user_status", "pending,active,suspended,banned")
                .OldAnnotation("Npgsql:Enum:notification.notif_channel", "push,sms,email,in_app")
                .OldAnnotation("Npgsql:Enum:notification.notif_status", "pending,sent,failed,skipped")
                .OldAnnotation("Npgsql:Enum:ops.damage_severity", "minor,moderate,severe")
                .OldAnnotation("Npgsql:Enum:ops.damage_status", "reported,assessed,resolved,disputed")
                .OldAnnotation("Npgsql:Enum:ops.penalty_status", "pending,charged,disputed,waived")
                .OldAnnotation("Npgsql:Enum:ops.penalty_type", "traffic_fine,toll_violation,late_return,fuel_shortage,cleaning_fee")
                .OldAnnotation("Npgsql:Enum:payment.payment_direction", "charge,refund,payout")
                .OldAnnotation("Npgsql:Enum:payment.payment_method", "bank_transfer,momo,vnpay,zalopay,credit_card")
                .OldAnnotation("Npgsql:Enum:payment.payment_status", "pending,success,failed,refunded")
                .OldAnnotation("Npgsql:PostgresExtension:pgcrypto", ",,");

            migrationBuilder.AddColumn<string>(
                name: "driver_license_url",
                schema: "identity",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "identity_card_url",
                schema: "identity",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "kyc_reject_reason",
                schema: "identity",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<KycStatus>(
                name: "kyc_status",
                schema: "identity",
                table: "users",
                type: "identity.kyc_status",
                nullable: false,
                defaultValue: KycStatus.None);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "driver_license_url",
                schema: "identity",
                table: "users");

            migrationBuilder.DropColumn(
                name: "identity_card_url",
                schema: "identity",
                table: "users");

            migrationBuilder.DropColumn(
                name: "kyc_reject_reason",
                schema: "identity",
                table: "users");

            migrationBuilder.DropColumn(
                name: "kyc_status",
                schema: "identity",
                table: "users");

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
                .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,")
                .OldAnnotation("Npgsql:Enum:booking.booking_status", "pending,confirmed,active,completed,cancelled,disputed")
                .OldAnnotation("Npgsql:Enum:booking.promotion_type", "percentage,fixed_amount")
                .OldAnnotation("Npgsql:Enum:booking.saga_status", "started,succeeded,failed,compensating")
                .OldAnnotation("Npgsql:Enum:fleet.car_status", "available,rented,maintenance,inactive")
                .OldAnnotation("Npgsql:Enum:fleet.fuel_type", "gasoline,diesel,electric,hybrid")
                .OldAnnotation("Npgsql:Enum:fleet.rental_type", "hourly,daily")
                .OldAnnotation("Npgsql:Enum:fleet.transmission_type", "automatic,manual")
                .OldAnnotation("Npgsql:Enum:identity.kyc_status", "none,pending,approved,rejected")
                .OldAnnotation("Npgsql:Enum:identity.otp_purpose", "login,register,reset_password,verify_phone")
                .OldAnnotation("Npgsql:Enum:identity.provider_name", "google,facebook,zalo,apple")
                .OldAnnotation("Npgsql:Enum:identity.user_status", "pending,active,suspended,banned")
                .OldAnnotation("Npgsql:Enum:notification.notif_channel", "push,sms,email,in_app")
                .OldAnnotation("Npgsql:Enum:notification.notif_status", "pending,sent,failed,skipped")
                .OldAnnotation("Npgsql:Enum:ops.damage_severity", "minor,moderate,severe")
                .OldAnnotation("Npgsql:Enum:ops.damage_status", "reported,assessed,resolved,disputed")
                .OldAnnotation("Npgsql:Enum:ops.penalty_status", "pending,charged,disputed,waived")
                .OldAnnotation("Npgsql:Enum:ops.penalty_type", "traffic_fine,toll_violation,late_return,fuel_shortage,cleaning_fee")
                .OldAnnotation("Npgsql:Enum:payment.payment_direction", "charge,refund,payout")
                .OldAnnotation("Npgsql:Enum:payment.payment_method", "bank_transfer,momo,vnpay,zalopay,credit_card")
                .OldAnnotation("Npgsql:Enum:payment.payment_status", "pending,success,failed,refunded")
                .OldAnnotation("Npgsql:PostgresExtension:pgcrypto", ",,");
        }
    }
}
