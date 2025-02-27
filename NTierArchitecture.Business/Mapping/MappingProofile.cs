using System;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NTierArchitecture.Business.Features.Category.Create;
using NTierArchitecture.Business.Features.Products.CreateProduct;
using NTierArchitecture.Entities.Models;

namespace NTierArchitecture.Business.Mapping;

    internal sealed class MappingProofile: Profile
    {
        public MappingProofile()
        {
            CreateMap<CreateProductCommand, Product>();
            CreateMap<CreateCategoryCommand, Category>();
        }
    }

