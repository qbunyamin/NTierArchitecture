
using MediatR;
using NTierArchitecture.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace NTierArchitecture.Business.Features.Category.Update;
internal sealed class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        Entities.Models.Category category =
            await _categoryRepository.GetByIdAsync(p => p.Id == request.Id, cancellationToken);
        if (category is null)
        {
            throw new ArgumentException("Bu katogori güncellenmiş...");
        }

        if (category.Name != request.Name)
        {
            var isCategoryNameExist = await _categoryRepository.AnyAsync(p => p.Name == request.Name, cancellationToken);

            if (isCategoryNameExist)
            {
                throw new ArgumentException("Bu katogori eklenmiş...");
            }

           _mapper.Map(request, category);//değişen alanları set eder

            await _unitOfWork.SaveChangesAsync(cancellationToken);

        }


    }
}