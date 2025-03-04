using MediatR;

namespace NTierArchitecture.Entities.Events.Users;

public sealed class SendRegisterSms : INotificationHandler<UserDomainEvent>
{
    public Task Handle(UserDomainEvent notification, CancellationToken cancellationToken)
    {
        // sms gönderme
        //notification.AppUser.Id
        return Task.CompletedTask;
    }
}