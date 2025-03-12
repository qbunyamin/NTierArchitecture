using MediatR;
using NTierArchitecture.Business.Behaviors;
using NTierArchitecture.Entities.Models;

namespace NTierArchitecture.Business.Features.Products.GetProduct;
public sealed record GetProductsQuery() : IRequest<List<Product>>;