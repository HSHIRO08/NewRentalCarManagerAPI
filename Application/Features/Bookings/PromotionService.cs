using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewRentalCarManagerAPI.Common;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Models;
using System.Net;

namespace NewRentalCarManagerAPI.Application.Features.Bookings;

public interface IPromotionService
{
    Task<DataResult> GetAllAsync();
    Task<DataResult> GetByIdAsync(Guid id);
    Task<DataResult> GetByCodeAsync(string code);
    Task<DataResult> CreateAsync(CreatePromotionDto dto);
    Task<DataResult> UpdateAsync(Guid id, UpdatePromotionDto dto);
    Task<DataResult> DeleteAsync(Guid id);
}

public class PromotionService : IPromotionService
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<PromotionService> _logger;

    public PromotionService(IUnitOfWork uow, ILogger<PromotionService> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<DataResult> GetAllAsync()
    {
        try
        {
            var items = await _uow.Promotions.GetAllAsync();
            var data = items.Select(MapToDto).ToList();
            return DataResult.ResultSuccess(data, "Get success!", data.Count);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get promotions");
            throw;
        }
    }

    public async Task<DataResult> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await _uow.Promotions.GetByIdAsync(id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Promotion not found!");
            return DataResult.ResultSuccess(MapToDto(entity), "Get success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get promotion {Id}", id);
            throw;
        }
    }

    public async Task<DataResult> GetByCodeAsync(string code)
    {
        try
        {
            var entity = await _uow.Promotions.Query().FirstOrDefaultAsync(p => p.Code == code && p.IsActive)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Promotion not found!");
            return DataResult.ResultSuccess(MapToDto(entity), "Get success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get promotion by code {Code}", code);
            throw;
        }
    }

    public async Task<DataResult> CreateAsync(CreatePromotionDto dto)
    {
        try
        {
            var entity = new Promotion
            {
                Code = dto.Code,
                DiscountValue = dto.DiscountValue,
                MaxDiscountVnd = dto.MaxDiscountVnd,
                MinBookingVnd = dto.MinBookingVnd,
                UsageLimit = dto.UsageLimit,
                UsedCount = 0,
                ValidFrom = dto.ValidFrom,
                ValidTo = dto.ValidTo,
                IsActive = true
            };
            await _uow.Promotions.AddAsync(entity);
            return DataResult.ResultSuccess(MapToDto(entity), "Insert success!", statusCode: 201);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create promotion");
            throw;
        }
    }

    public async Task<DataResult> UpdateAsync(Guid id, UpdatePromotionDto dto)
    {
        try
        {
            var entity = await _uow.Promotions.GetByIdAsync(id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Promotion not found!");
            entity.DiscountValue = dto.DiscountValue;
            entity.MaxDiscountVnd = dto.MaxDiscountVnd;
            entity.MinBookingVnd = dto.MinBookingVnd;
            entity.UsageLimit = dto.UsageLimit;
            entity.ValidFrom = dto.ValidFrom;
            entity.ValidTo = dto.ValidTo;
            entity.IsActive = dto.IsActive;
            return DataResult.ResultSuccess(MapToDto(entity), "Update success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to update promotion {Id}", id);
            throw;
        }
    }

    public async Task<DataResult> DeleteAsync(Guid id)
    {
        try
        {
            var entity = await _uow.Promotions.GetByIdAsync(id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Promotion not found!");
            _uow.Promotions.Remove(entity);
            return DataResult.ResultSuccess(true, "Delete success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to delete promotion {Id}", id);
            throw;
        }
    }

    private static PromotionDto MapToDto(Promotion e) => new()
    {
        Id = e.Id,
        Code = e.Code,
        DiscountValue = e.DiscountValue,
        MaxDiscountVnd = e.MaxDiscountVnd,
        MinBookingVnd = e.MinBookingVnd,
        UsageLimit = e.UsageLimit,
        UsedCount = e.UsedCount,
        ValidFrom = e.ValidFrom,
        ValidTo = e.ValidTo,
        IsActive = e.IsActive
    };
}
