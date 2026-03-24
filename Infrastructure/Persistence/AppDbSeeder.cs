using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NewRentalCarManagerAPI.Models;

namespace NewRentalCarManagerAPI.Infrastructure.Persistence;

public static class AppDbSeeder
{
    public static void SeedDefaultRolesAndPermissions(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        string[] defaultRoles = ["admin", "owner", "renter"];
        foreach (var roleName in defaultRoles)
        {
            if (!db.Roles.Any(r => r.Name == roleName))
            {
                db.Roles.Add(new Role
                {
                    Id = Guid.NewGuid(),
                    Name = roleName,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }
        db.SaveChanges();

        string[] resources = ["users", "cars", "bookings", "promotions", "reviews",
                              "penalties", "damage_reports", "transactions", "owner_payouts",
                              "car_brands", "car_models", "locations", "car_pricing"];
        string[] actions = ["read", "create", "update", "delete"];

        foreach (var resource in resources)
        foreach (var action in actions)
        {
            if (!db.Permissions.Any(p => p.Resource == resource && p.Action == action))
            {
                db.Permissions.Add(new Permission
                {
                    Id = Guid.NewGuid(),
                    Resource = resource,
                    Action = action
                });
            }
        }
        db.SaveChanges();

        var adminRole = db.Roles.Include(r => r.Permissions).First(r => r.Name == "admin");
        var ownerRole = db.Roles.Include(r => r.Permissions).First(r => r.Name == "owner");
        var renterRole = db.Roles.Include(r => r.Permissions).First(r => r.Name == "renter");
        var allPermissions = db.Permissions.ToList();

        foreach (var perm in allPermissions)
        {
            if (!adminRole.Permissions.Any(p => p.Id == perm.Id))
            {
                adminRole.Permissions.Add(perm);
            }
        }

        var ownerReadResources = new[] { "cars", "bookings", "reviews", "car_pricing", "car_brands", "car_models", "locations" };
        var ownerManageResources = new[] { "cars", "bookings", "reviews", "owner_payouts" };

        foreach (var perm in allPermissions.Where(p => ownerReadResources.Contains(p.Resource) && p.Action == "read"))
        {
            if (!ownerRole.Permissions.Any(p => p.Id == perm.Id))
            {
                ownerRole.Permissions.Add(perm);
            }
        }

        foreach (var perm in allPermissions.Where(p => ownerManageResources.Contains(p.Resource) && new[] { "create", "update", "delete" }.Contains(p.Action)))
        {
            if (!ownerRole.Permissions.Any(p => p.Id == perm.Id))
            {
                ownerRole.Permissions.Add(perm);
            }
        }

        var renterReadResources = new[] { "bookings", "reviews", "cars", "car_pricing", "locations" };

        foreach (var perm in allPermissions.Where(p => renterReadResources.Contains(p.Resource) && p.Action == "read"))
        {
            if (!renterRole.Permissions.Any(p => p.Id == perm.Id))
            {
                renterRole.Permissions.Add(perm);
            }
        }

        foreach (var perm in allPermissions.Where(p => p.Resource == "bookings" && new[] { "create", "update", "delete" }.Contains(p.Action)))
        {
            if (!renterRole.Permissions.Any(p => p.Id == perm.Id))
            {
                renterRole.Permissions.Add(perm);
            }
        }

        foreach (var perm in allPermissions.Where(p => p.Resource == "reviews" && p.Action == "create"))
        {
            if (!renterRole.Permissions.Any(p => p.Id == perm.Id))
            {
                renterRole.Permissions.Add(perm);
            }
        }

        db.SaveChanges();
    }
}
