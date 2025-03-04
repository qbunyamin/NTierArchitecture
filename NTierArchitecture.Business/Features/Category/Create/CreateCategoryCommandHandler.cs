
using MediatR;
using NTierArchitecture.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ErrorOr;

namespace NTierArchitecture.Business.Features.Category.Create;
internal sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand ,ErrorOr<Unit>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ErrorOr<Unit>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var isCategoryNameExist = await _categoryRepository.AnyAsync(p => p.Name == request.Name, cancellationToken);

        if (isCategoryNameExist)
        {
            return Error.Conflict("NameIsExists","Bu kategori daha önce oluşturulmuş");
        }

        Entities.Models.Category category = _mapper.Map<Entities.Models.Category>(request);
        await _categoryRepository.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
