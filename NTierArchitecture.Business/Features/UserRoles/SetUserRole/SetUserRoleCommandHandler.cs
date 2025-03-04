using MediatR;
using NTierArchitecture.Entities.Models;
using NTierArchitecture.Entities.Repositories;

namespace NTierArchitecture.Business.Features.UserRoles.SetUserRole;

internal sealed class SetUserRoleCommandHandler : IRequestHandler<SetUserRoleCommand, Unit>
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SetUserRoleCommandHandler(IUserRoleRepository userRoleRepository, IUnitOfWork unitOfWork)
    {
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(SetUserRoleCommand request, CancellationToken cancellationToken)
    {
        var checkIsROleExists =
            await _userRoleRepository
                .AnyAsync(p => p.AppUserId == request.UserId && p.AppRoleId == request.RoleId,cancellationToken);
        if (checkIsROleExists)
        {

            throw new ArgumentException("Bu rol daha önce eklenmiş.");

        }

        UserRole userRole = new()
        {
            AppRoleId = request.RoleId,
            AppUserId = request.UserId,
        };

        await _userRoleRepository.AddAsync(userRole, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}