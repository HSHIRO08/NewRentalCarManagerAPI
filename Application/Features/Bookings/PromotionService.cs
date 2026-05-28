using AutoMapper;
using AutoMapper.QueryableExtensions;
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
    private readonly IRepository<Promotion> _promotionRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PromotionService> _logger;

    public PromotionService(IRepository<Promotion> promotionRepository, IMapper mapper, ILogger<PromotionService> logger)
    {
        _promotionRepository = promotionRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<DataResult> GetAllAsync()
    {
        try
        {
            var items = await _promotionRepository.Query().ProjectTo<PromotionDto>(_mapper.ConfigurationProvider).ToListAsync();
            return DataResult.ResultSuccess(items, "Get success!", items.Count);
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
            var entity = await _promotionRepository.GetByIdAsync(id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Promotion not found!");
            return DataResult.ResultSuccess(_mapper.Map<PromotionDto>(entity), "Get success!");
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
            var entity = await _promotionRepository.Query().FirstOrDefaultAsync(p => p.Code == code && p.IsActive)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Promotion not found!");
            return DataResult.ResultSuccess(_mapper.Map<PromotionDto>(entity), "Get success!");
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
            await _promotionRepository.AddAsync(entity);
            return DataResult.ResultSuccess(_mapper.Map<PromotionDto>(entity), "Insert success!", statusCode: 201);
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
            var entity = await _promotionRepository.GetByIdAsync(id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Promotion not found!");
            entity.DiscountValue = dto.DiscountValue;
            entity.MaxDiscountVnd = dto.MaxDiscountVnd;
            entity.MinBookingVnd = dto.MinBookingVnd;
            entity.UsageLimit = dto.UsageLimit;
            entity.ValidFrom = dto.ValidFrom;
            entity.ValidTo = dto.ValidTo;
            entity.IsActive = dto.IsActive;
            return DataResult.ResultSuccess(_mapper.Map<PromotionDto>(entity), "Update success!");
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
            var entity = await _promotionRepository.GetByIdAsync(id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Promotion not found!");
            _promotionRepository.Remove(entity);
            return DataResult.ResultSuccess(true, "Delete success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to delete promotion {Id}", id);
            throw;
        }
    }

}
