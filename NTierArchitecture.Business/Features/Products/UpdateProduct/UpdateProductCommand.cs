using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace NTierArchitecture.Business.Features.Products.UpdateProduct;

public sealed record UpdateProductCommand(
    string Name,
    decimal Price,
    int Quantity,
    Guid CategoryId) : IRequest;