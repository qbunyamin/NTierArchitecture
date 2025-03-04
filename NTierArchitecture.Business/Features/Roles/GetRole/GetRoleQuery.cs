using MediatR;

namespace NTierArchitecture.Business.Features.Roles.GetRole;

public sealed record GetRoleQuery() : IRequest<List<GetRolesQueryResponse>>;