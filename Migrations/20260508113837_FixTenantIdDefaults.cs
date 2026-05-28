using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewRentalCarManagerAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixTenantIdDefaults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE fleet.cars SET tenant_id = 1 WHERE tenant_id IS NULL;
UPDATE fleet.car_pricing SET tenant_id = 1 WHERE tenant_id IS NULL;
UPDATE fleet.car_models SET tenant_id = 1 WHERE tenant_id IS NULL;
UPDATE fleet.car_brands SET tenant_id = 1 WHERE tenant_id IS NULL;
UPDATE fleet.locations SET tenant_id = 1 WHERE tenant_id IS NULL;
UPDATE fleet.car_availability_blocks SET tenant_id = 1 WHERE tenant_id IS NULL;

ALTER TABLE fleet.cars ALTER COLUMN tenant_id SET DEFAULT 1;
ALTER TABLE fleet.car_pricing ALTER COLUMN tenant_id SET DEFAULT 1;
ALTER TABLE fleet.car_models ALTER COLUMN tenant_id SET DEFAULT 1;
ALTER TABLE fleet.car_brands ALTER COLUMN tenant_id SET DEFAULT 1;
ALTER TABLE fleet.locations ALTER COLUMN tenant_id SET DEFAULT 1;
ALTER TABLE fleet.car_availability_blocks ALTER COLUMN tenant_id SET DEFAULT 1;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
ALTER TABLE fleet.cars ALTER COLUMN tenant_id DROP DEFAULT;
ALTER TABLE fleet.car_pricing ALTER COLUMN tenant_id DROP DEFAULT;
ALTER TABLE fleet.car_models ALTER COLUMN tenant_id DROP DEFAULT;
ALTER TABLE fleet.car_brands ALTER COLUMN tenant_id DROP DEFAULT;
ALTER TABLE fleet.locations ALTER COLUMN tenant_id DROP DEFAULT;
ALTER TABLE fleet.car_availability_blocks ALTER COLUMN tenant_id DROP DEFAULT;
");
        }
    }
}
