using MediatR;
using Microsoft.EntityFrameworkCore;
using NTierArchitecture.Entities.Repositories;

namespace NTierArchitecture.Business.Features.Category.Get;
internal sealed class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<Entities.Models.Category>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoriesQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<Entities.Models.Category>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await _categoryRepository.GetAll().OrderBy(p => p.Name).ToListAsync(cancellationToken);
    }
}
