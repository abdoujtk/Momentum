using MediatR;
using Momentum.Application.Common;
using Momentum.Application.Interfaces;
using Momentum.Contracts.DTOs;

namespace Momentum.Application.UseCases.Categories.GetCategories;

public sealed record GetCategoriesQuery : IRequest<Result<List<CategoryDto>>>;

public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, Result<List<CategoryDto>>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoriesHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<List<CategoryDto>>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);

        var dtos = categories.Select(c => new CategoryDto(
            c.CategoryId,
            c.Name,
            c.ColorHex
        )).ToList();

        return Result<List<CategoryDto>>.Success(dtos);
    }
}