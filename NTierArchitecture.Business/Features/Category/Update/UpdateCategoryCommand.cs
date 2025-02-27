using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using NTierArchitecture.Entities.Repositories;

namespace NTierArchitecture.Business.Features.Category.Update;

public sealed record UpdateCategoryCommand
    (Guid Id, string Name):IRequest;



