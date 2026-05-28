using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewRentalCarManagerAPI.Common;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Models;
using System.Net;

namespace NewRentalCarManagerAPI.Application.Features.Ops;

public interface IPenaltyService
{
    Task<DataResult> GetByBookingAsync(Guid bookingId, OpsListInput input);
    Task<DataResult> GetByIdAsync(Guid id);
    Task<DataResult> CreateAsync(CreatePenaltyDto dto);
}

public class PenaltyService : IPenaltyService
{
    private readonly IRepository<Penalty> _penaltyRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PenaltyService> _logger;

    public PenaltyService(IRepository<Penalty> penaltyRepository, IMapper mapper, ILogger<PenaltyService> logger)
    {
        _penaltyRepository = penaltyRepository;
        _mapper = mapper;
        _logger = logger;
    }

    private IQueryable<Penalty> BaseQuery() => _penaltyRepository.Query()
        .Include(p => p.ChargedToNavigation);

    public async Task<DataResult> GetByBookingAsync(Guid bookingId, OpsListInput input)
    {
        try
        {
            var query = BaseQuery().Where(p => p.BookingId == bookingId).OrderByDescending(p => p.CreatedAt);
            var totalCount = await query.CountAsync();
            var items = await query.Skip(input.SkipCount).Take(input.MaxResultCount)
                .ProjectTo<PenaltyDto>(_mapper.ConfigurationProvider).ToListAsync();
            return DataResult.ResultSuccess(items, "Get success!", totalCount);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get penalties by booking {BookingId}", bookingId);
            throw;
        }
    }

    public async Task<DataResult> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await BaseQuery().FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Penalty not found!");
            return DataResult.ResultSuccess(_mapper.Map<PenaltyDto>(entity), "Get success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get penalty {Id}", id);
            throw;
        }
    }

    public async Task<DataResult> CreateAsync(CreatePenaltyDto dto)
    {
        try
        {
            var entity = new Penalty
            {
                BookingId = dto.BookingId,
                ChargedTo = dto.ChargedTo,
                AmountVnd = dto.AmountVnd,
                Description = dto.Description,
                EvidenceUrl = dto.EvidenceUrl,
                CreatedAt = DateTime.UtcNow
            };
            await _penaltyRepository.AddAsync(entity);
            var created = await BaseQuery().FirstOrDefaultAsync(p => p.Id == entity.Id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.InternalServerError, "Create penalty failed.");
            return DataResult.ResultSuccess(_mapper.Map<PenaltyDto>(created), "Insert success!", statusCode: 201);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create penalty");
            throw;
        }
    }

}
