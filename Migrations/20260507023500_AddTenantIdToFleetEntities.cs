using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewRentalCarManagerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantIdToFleetEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "tenant_id",
                schema: "fleet",
                table: "locations",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "tenant_id",
                schema: "fleet",
                table: "cars",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "tenant_id",
                schema: "fleet",
                table: "car_pricing",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "tenant_id",
                schema: "fleet",
                table: "car_models",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "tenant_id",
                schema: "fleet",
                table: "car_brands",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "tenant_id",
                schema: "fleet",
                table: "car_availability_blocks",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tenant_id",
                schema: "fleet",
                table: "locations");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                schema: "fleet",
                table: "cars");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                schema: "fleet",
                table: "car_pricing");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                schema: "fleet",
                table: "car_models");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                schema: "fleet",
                table: "car_brands");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                schema: "fleet",
                table: "car_availability_blocks");
        }
    }
}
