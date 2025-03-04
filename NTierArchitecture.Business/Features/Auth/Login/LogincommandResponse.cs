namespace NTierArchitecture.Business.Features.Auth.Login;

public sealed record LogincommandResponse(string AccessToken,
    Guid UserId);