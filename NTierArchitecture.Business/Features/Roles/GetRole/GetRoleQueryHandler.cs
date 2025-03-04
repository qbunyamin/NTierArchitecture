using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NTierArchitecture.Entities.Models;
using NTierArchitecture.Entities.Repositories;

namespace NTierArchitecture.Business.Features.Roles.GetRole;

internal sealed class GetRoleQueryHandler : IRequestHandler<GetRoleQuery, List<GetRolesQueryResponse>>
{
    private readonly IRoleRepository _roleRepository;


    public GetRoleQueryHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;

    }
    public async Task<List<GetRolesQueryResponse>> Handle(GetRoleQuery request, CancellationToken cancellationToken)
    {
        var response =
            await _roleRepository.GetAll()
                .Select(r => new GetRolesQueryResponse(r.Id, r.Name))
                .ToListAsync(cancellationToken);

        return response;
    }
}