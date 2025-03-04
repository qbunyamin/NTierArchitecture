using MediatR;
using Microsoft.AspNetCore.Identity;
using NTierArchitecture.Entities.Events.Users;
using NTierArchitecture.Entities.Models;

namespace NTierArchitecture.Business.Features.Auth.Register;

internal sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Unit>
{

    private readonly UserManager<AppUser> _userManager;
    private readonly IMediator _mediator;

    public RegisterCommandHandler(UserManager<AppUser> userManager, IMediator mediator)
    {
        _userManager = userManager;
        _mediator = mediator;
    }

    public async Task<Unit> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var checkUserNameExists = await _userManager.FindByNameAsync(request.UserName);
        if (checkUserNameExists is not null)
        {
            throw new ArgumentException("Kullanıcı adı mevcut.");
        }
        var checkEmailExists = await _userManager.FindByNameAsync(request.Email);
        if (checkEmailExists is not null)
        {
            throw new ArgumentException("Email adı mevcut.");
        }

        AppUser appUser = new()
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Name = request.name,
            Lastname = request.lastName,
            UserName = request.UserName
        };

        await _userManager.CreateAsync(appUser, request.Password);
        await _mediator.Publish(new UserDomainEvent(appUser));
        return Unit.Value;

    }
}