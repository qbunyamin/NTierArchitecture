using MediatR;
using NTierArchitecture.Entities.Repositories;

namespace NTierArchitecture.Business.Features.Category.Create;

public sealed record CreateCategoryCommand(
    string Name) : IRequest;



