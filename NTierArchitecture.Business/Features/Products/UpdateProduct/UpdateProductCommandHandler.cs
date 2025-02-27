using AutoMapper;
using MediatR;
using NTierArchitecture.Entities.Models;
using NTierArchitecture.Entities.Repositories;

namespace NTierArchitecture.Business.Features.Products.UpdateProduct;

internal sealed class UpdateProductCommandHandler :IRequestHandler<UpdateProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        Product product = await _productRepository.GetByIdAsync(p => p.Id == request.CategoryId, cancellationToken);
        if (product is null)
        {
            throw new ArgumentException("Ürün bulunamadı");
        }

        if (product.Name!= request.Name)
        {
            var isCategoryNameExist = await _productRepository.AnyAsync(p => p.Name == request.Name, cancellationToken);

            if (isCategoryNameExist)
            {
                throw new ArgumentException("Bu katogori eklenmiş...");
            }

            _mapper.Map(request, product);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}