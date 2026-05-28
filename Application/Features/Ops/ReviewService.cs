using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewRentalCarManagerAPI.Common;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Models;
using System.Net;

namespace NewRentalCarManagerAPI.Application.Features.Ops;

public interface IReviewService
{
    Task<DataResult> GetByBookingAsync(Guid bookingId, OpsListInput input);
    Task<DataResult> GetByCarAsync(Guid carId, OpsListInput input);
    Task<DataResult> GetByIdAsync(Guid id);
    Task<DataResult> CreateAsync(CreateReviewDto dto);
    Task<DataResult> DeleteAsync(Guid id);
}

public class ReviewService : IReviewService
{
    private readonly IRepository<Review> _reviewRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ReviewService> _logger;

    public ReviewService(IRepository<Review> reviewRepository, IMapper mapper, ILogger<ReviewService> logger)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
        _logger = logger;
    }

    private IQueryable<Review> BaseQuery() => _reviewRepository.Query()
        .Include(r => r.Reviewer)
        .Include(r => r.Reviewee);

    public async Task<DataResult> GetByBookingAsync(Guid bookingId, OpsListInput input)
    {
        try
        {
            var query = BaseQuery().Where(r => r.BookingId == bookingId).OrderByDescending(r => r.CreatedAt);
            var totalCount = await query.CountAsync();
            var items = await query.Skip(input.SkipCount).Take(input.MaxResultCount)
                .ProjectTo<ReviewDto>(_mapper.ConfigurationProvider).ToListAsync();
            return DataResult.ResultSuccess(items, "Get success!", totalCount);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get reviews by booking {BookingId}", bookingId);
            throw;
        }
    }

    public async Task<DataResult> GetByCarAsync(Guid carId, OpsListInput input)
    {
        try
        {
            var query = BaseQuery().Where(r => r.CarId == carId).OrderByDescending(r => r.CreatedAt);
            var totalCount = await query.CountAsync();
            var items = await query.Skip(input.SkipCount).Take(input.MaxResultCount)
                .ProjectTo<ReviewDto>(_mapper.ConfigurationProvider).ToListAsync();
            return DataResult.ResultSuccess(items, "Get success!", totalCount);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get reviews by car {CarId}", carId);
            throw;
        }
    }

    public async Task<DataResult> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await BaseQuery().FirstOrDefaultAsync(r => r.Id == id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Review not found!");
            return DataResult.ResultSuccess(_mapper.Map<ReviewDto>(entity), "Get success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get review {Id}", id);
            throw;
        }
    }

    public async Task<DataResult> CreateAsync(CreateReviewDto dto)
    {
        try
        {
            var entity = new Review
            {
                BookingId = dto.BookingId,
                ReviewerId = dto.ReviewerId,
                RevieweeId = dto.RevieweeId,
                CarId = dto.CarId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };
            await _reviewRepository.AddAsync(entity);
            var created = await BaseQuery().FirstOrDefaultAsync(r => r.Id == entity.Id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.InternalServerError, "Create review failed.");
            return DataResult.ResultSuccess(_mapper.Map<ReviewDto>(created), "Insert success!", statusCode: 201);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create review");
            throw;
        }
    }

    public async Task<DataResult> DeleteAsync(Guid id)
    {
        try
        {
            var entity = await _reviewRepository.GetByIdAsync(id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Review not found!");
            _reviewRepository.Remove(entity);
            return DataResult.ResultSuccess(true, "Delete success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to delete review {Id}", id);
            throw;
        }
    }

}
