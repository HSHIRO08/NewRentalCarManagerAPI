using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NewRentalCarManagerAPI.Enums;

#nullable disable

namespace NewRentalCarManagerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddHandoverRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:booking.booking_status", "pending,confirmed,active,completed,cancelled,disputed")
                .Annotation("Npgsql:Enum:booking.handover_type", "check_in,check_out")
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

            migrationBuilder.CreateTable(
                name: "handover_records",
                schema: "booking",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    booking_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<HandoverType>(type: "booking.handover_type", nullable: false),
                    mileage_km = table.Column<int>(type: "integer", nullable: true),
                    fuel_percent = table.Column<int>(type: "integer", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    image_urls = table.Column<List<string>>(type: "text[]", nullable: true),
                    recorded_by = table.Column<Guid>(type: "uuid", nullable: false),
                    recorded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("handover_records_pkey", x => x.id);
                    table.ForeignKey(
                        name: "handover_records_booking_id_fkey",
                        column: x => x.booking_id,
                        principalSchema: "booking",
                        principalTable: "bookings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "handover_records_recorded_by_fkey",
                        column: x => x.recorded_by,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "handover_records_booking_id_idx",
                schema: "booking",
                table: "handover_records",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "IX_handover_records_recorded_by",
                schema: "booking",
                table: "handover_records",
                column: "recorded_by");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "handover_records",
                schema: "booking");

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
                .OldAnnotation("Npgsql:Enum:booking.handover_type", "check_in,check_out")
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
