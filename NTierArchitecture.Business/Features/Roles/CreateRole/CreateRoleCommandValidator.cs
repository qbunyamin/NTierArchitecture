using FluentValidation;

namespace NTierArchitecture.Business.Features.Roles.CreateRole;

public sealed class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotNull().WithMessage("Rol adı boş olamaz.")
            .NotEmpty().WithMessage("Rol adı boş olamaz.")
            .MinimumLength(3).WithMessage("Rol adı en az 3 karakter olmalıdır.");
    }
}