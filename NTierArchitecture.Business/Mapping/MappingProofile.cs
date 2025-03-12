using AutoMapper;
using NTierArchitecture.Business.Features.Category.Create;
using NTierArchitecture.Business.Features.Category.Update;
using NTierArchitecture.Business.Features.Products.CreateProduct;
using NTierArchitecture.Business.Features.Products.UpdateProduct;
using NTierArchitecture.Entities.Models;

namespace NTierArchitecture.Business.Mapping;

internal sealed class MappingProofile : Profile
{
    public MappingProofile()
    {
        CreateMap<CreateProductCommand, Product>();
        CreateMap<UpdateProductCommand, Product>();
        CreateMap<CreateCategoryCommand, Category>();
        CreateMap<UpdateCategoryCommand, Category>();
    }
}

