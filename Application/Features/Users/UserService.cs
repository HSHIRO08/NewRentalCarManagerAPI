using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewRentalCarManagerAPI.Common;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Models;
using System.Net;

namespace NewRentalCarManagerAPI.Application.Features.Users;

public interface IUserService
{
    Task<DataResult> GetAllAsync();
    Task<DataResult> GetByIdAsync(Guid id);
    Task<DataResult> CreateAsync(CreateUserDto dto);
    Task<DataResult> UpdateAsync(Guid id, UpdateUserDto dto);
    Task<DataResult> DeleteAsync(Guid id);
}

public class UserService : IUserService
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<UserService> _logger;
    public UserService(IUnitOfWork uow, ILogger<UserService> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<DataResult> GetAllAsync()
    {
        try
        {
            var items = await _uow.Users.Query().Include(u => u.Role).ToListAsync();
            return DataResult.ResultSuccess(items.Select(MapToDto).ToList(), "Get success!", items.Count);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get users");
            throw;
        }
    }

    public async Task<DataResult> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await _uow.Users.Query().Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "User not found!");
            return DataResult.ResultSuccess(MapToDto(entity), "Get success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get user {Id}", id);
            throw;
        }
    }

    public async Task<DataResult> CreateAsync(CreateUserDto dto)
    {
        try
        {
            var entity = new User
            {
                Phone = dto.Phone, Email = dto.Email, PasswordHash = dto.PasswordHash,
                FullName = dto.FullName, AvatarUrl = dto.AvatarUrl, RoleId = dto.RoleId,
                ReferralCode = dto.ReferralCode, ReferredById = dto.ReferredById,
                CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            };
            await _uow.Users.AddAsync(entity);
            var created = await _uow.Users.Query().Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == entity.Id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.InternalServerError, "Create user failed.");
            return DataResult.ResultSuccess(MapToDto(created), "Insert success!", statusCode: 201);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create user");
            throw;
        }
    }

    public async Task<DataResult> UpdateAsync(Guid id, UpdateUserDto dto)
    {
        try
        {
            var entity = await _uow.Users.Query().Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "User not found!");
            entity.Email = dto.Email;
            entity.FullName = dto.FullName;
            entity.AvatarUrl = dto.AvatarUrl;
            entity.UpdatedAt = DateTime.UtcNow;
            return DataResult.ResultSuccess(MapToDto(entity), "Update success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to update user {Id}", id);
            throw;
        }
    }

    public async Task<DataResult> DeleteAsync(Guid id)
    {
        try
        {
            var entity = await _uow.Users.GetByIdAsync(id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "User not found!");
            _uow.Users.Remove(entity);
            return DataResult.ResultSuccess(true, "Delete success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to delete user {Id}", id);
            throw;
        }
    }

    private static UserDto MapToDto(User e) => new()
    {
        Id = e.Id, Phone = e.Phone, Email = e.Email, FullName = e.FullName,
        AvatarUrl = e.AvatarUrl, RoleId = e.RoleId, RoleName = e.Role.Name,
        Status = e.Status.ToString(),
        ReferralCode = e.ReferralCode, ReferredById = e.ReferredById,
        CreatedAt = e.CreatedAt, UpdatedAt = e.UpdatedAt
    };
}
