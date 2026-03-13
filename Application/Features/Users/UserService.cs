using Microsoft.EntityFrameworkCore;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Models;

namespace NewRentalCarManagerAPI.Application.Features.Users;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<UserDto> CreateAsync(CreateUserDto dto);
    Task<UserDto?> UpdateAsync(Guid id, UpdateUserDto dto);
    Task<bool> DeleteAsync(Guid id);
}

public class UserService : IUserService
{
    private readonly IUnitOfWork _uow;
    public UserService(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var items = await _uow.Users.Query().Include(u => u.Role).ToListAsync();
        return items.Select(MapToDto);
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var e = await _uow.Users.Query().Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);
        return e is null ? null : MapToDto(e);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        var e = new User
        {
            Phone = dto.Phone, Email = dto.Email, PasswordHash = dto.PasswordHash,
            FullName = dto.FullName, AvatarUrl = dto.AvatarUrl, RoleId = dto.RoleId,
            ReferralCode = dto.ReferralCode, ReferredById = dto.ReferredById,
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        };
        await _uow.Users.AddAsync(e);
        return (await GetByIdAsync(e.Id))!;
    }

    public async Task<UserDto?> UpdateAsync(Guid id, UpdateUserDto dto)
    {
        var e = await _uow.Users.Query().Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);
        if (e is null) return null;
        e.Email = dto.Email; e.FullName = dto.FullName;
        e.AvatarUrl = dto.AvatarUrl; e.UpdatedAt = DateTime.UtcNow;
        return MapToDto(e);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var e = await _uow.Users.GetByIdAsync(id);
        if (e is null) return false;
        _uow.Users.Remove(e);
        return true;
    }

    private static UserDto MapToDto(User e) => new()
    {
        Id = e.Id, Phone = e.Phone, Email = e.Email, FullName = e.FullName,
        AvatarUrl = e.AvatarUrl, RoleId = e.RoleId, RoleName = e.Role.Name,
        ReferralCode = e.ReferralCode, ReferredById = e.ReferredById,
        CreatedAt = e.CreatedAt, UpdatedAt = e.UpdatedAt
    };
}
