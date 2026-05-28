using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewRentalCarManagerAPI.Common;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Enums;
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
    Task<DataResult> SubmitKycAsync(Guid userId, SubmitKycDto dto);
    Task<DataResult> ReviewKycAsync(Guid userId, ReviewKycDto dto);
}

public class UserService : IUserService
{
    private readonly IRepository<User> _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;
    public UserService(IRepository<User> userRepository, IMapper mapper, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<DataResult> GetAllAsync()
    {
        try
        {
            var items = await _userRepository.Query().ProjectTo<UserDto>(_mapper.ConfigurationProvider).ToListAsync();
            return DataResult.ResultSuccess(items, "Get success!", items.Count);
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
            var entity = await _userRepository.Query().Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "User not found!");
            return DataResult.ResultSuccess(_mapper.Map<UserDto>(entity), "Get success!");
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
            await _userRepository.AddAsync(entity);
            var created = await _userRepository.Query().Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == entity.Id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.InternalServerError, "Create user failed.");
            return DataResult.ResultSuccess(_mapper.Map<UserDto>(created), "Insert success!", statusCode: 201);
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
            var entity = await _userRepository.Query().Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "User not found!");
            entity.Email = dto.Email;
            entity.FullName = dto.FullName;
            entity.AvatarUrl = dto.AvatarUrl;
            if (!string.IsNullOrEmpty(dto.Status) && Enum.TryParse<UserStatus>(dto.Status, true, out var parsedStatus))
                entity.Status = parsedStatus;
            entity.UpdatedAt = DateTime.UtcNow;
            return DataResult.ResultSuccess(_mapper.Map<UserDto>(entity), "Update success!");
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
            var entity = await _userRepository.GetByIdAsync(id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "User not found!");
            _userRepository.Remove(entity);
            return DataResult.ResultSuccess(true, "Delete success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to delete user {Id}", id);
            throw;
        }
    }

    public async Task<DataResult> SubmitKycAsync(Guid userId, SubmitKycDto dto)
    {
        try
        {
            var entity = await _userRepository.GetByIdAsync(userId)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "User not found!");

            if (entity.KycStatus == KycStatus.Approved)
                throw new UserFriendlyException((int)HttpStatusCode.Conflict, "KYC đã được duyệt, không thể nộp lại.");

            entity.IdentityCardUrl = dto.IdentityCardUrl;
            entity.DriverLicenseUrl = dto.DriverLicenseUrl;
            entity.KycStatus = KycStatus.Pending;
            entity.KycRejectReason = null;
            entity.UpdatedAt = DateTime.UtcNow;

            var updated = await _userRepository.Query().Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId);
            return DataResult.ResultSuccess(_mapper.Map<UserDto>(updated), "KYC submitted successfully!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to submit KYC for user {UserId}", userId);
            throw;
        }
    }

    public async Task<DataResult> ReviewKycAsync(Guid userId, ReviewKycDto dto)
    {
        try
        {
            var entity = await _userRepository.GetByIdAsync(userId)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "User not found!");

            var action = dto.Action.Trim().ToLowerInvariant();
            if (action != "approve" && action != "reject")
                throw new UserFriendlyException((int)HttpStatusCode.BadRequest, "Action must be 'approve' or 'reject'.");

            if (entity.KycStatus != KycStatus.Pending)
                throw new UserFriendlyException((int)HttpStatusCode.Conflict, "Chỉ có thể review khi KYC đang Pending.");

            entity.KycStatus = action == "approve" ? KycStatus.Approved : KycStatus.Rejected;
            entity.KycRejectReason = action == "reject" ? dto.RejectReason : null;
            entity.UpdatedAt = DateTime.UtcNow;

            var updated = await _userRepository.Query().Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId);
            return DataResult.ResultSuccess(_mapper.Map<UserDto>(updated), "KYC reviewed!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to review KYC for user {UserId}", userId);
            throw;
        }
    }

}
