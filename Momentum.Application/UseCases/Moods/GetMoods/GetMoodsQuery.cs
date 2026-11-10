using MediatR;
using Momentum.Application.Common;
using Momentum.Application.Interfaces;
using Momentum.Contracts.DTOs;

namespace Momentum.Application.UseCases.Moods.GetMoods;

public sealed record GetMoodsQuery : IRequest<Result<List<MoodDto>>>;

public class GetMoodsHandler : IRequestHandler<GetMoodsQuery, Result<List<MoodDto>>>
{
    private readonly IMoodRepository _moodRepository;

    public GetMoodsHandler(IMoodRepository moodRepository)
        => _moodRepository = moodRepository;

    public async Task<Result<List<MoodDto>>> Handle(
        GetMoodsQuery request, CancellationToken ct)
    {
        var moods = await _moodRepository.GetAllAsync(ct);

        var dtos = moods.Select(m => new MoodDto(
            m.MoodId, m.Name, m.Emoji, m.Score, m.ColorHex)).ToList();

        return Result<List<MoodDto>>.Success(dtos);
    }
}