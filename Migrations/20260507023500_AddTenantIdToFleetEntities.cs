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
            migrationBuilder.Sql(@"
ALTER TABLE IF EXISTS fleet.locations ADD COLUMN IF NOT EXISTS tenant_id integer;
ALTER TABLE IF EXISTS fleet.cars ADD COLUMN IF NOT EXISTS tenant_id integer;
ALTER TABLE IF EXISTS fleet.car_pricing ADD COLUMN IF NOT EXISTS tenant_id integer;
ALTER TABLE IF EXISTS fleet.car_models ADD COLUMN IF NOT EXISTS tenant_id integer;
ALTER TABLE IF EXISTS fleet.car_brands ADD COLUMN IF NOT EXISTS tenant_id integer;
ALTER TABLE IF EXISTS fleet.car_availability_blocks ADD COLUMN IF NOT EXISTS tenant_id integer;
");
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
