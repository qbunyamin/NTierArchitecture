using FluentValidation;

namespace NTierArchitecture.Business.Features.Auth.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        //kullanıcı adı
        RuleFor(x => x.lastName)
            .NotEmpty().WithMessage("Kullanıcı adı boş olamaz.")
            .MinimumLength(5).WithMessage("Kullanıcı adı en az 5 karakter olmalıdır.")
            .MaximumLength(20).WithMessage("Kullanıcı adı en fazla 20 karakter olmalıdır.")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("Kullanıcı adı sadece harf, rakam ve alt çizgi içerebilir.");

        // şifre
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre boş olamaz.")
            .MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalıdır.")
            .MaximumLength(50).WithMessage("Şifre en fazla 50 karakter olmalıdır.")
            .Matches("[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir.")
            .Matches("[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir.")
            .Matches("[0-9]").WithMessage("Şifre en az bir rakam içermelidir.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Şifre en az bir özel karakter içermelidir.");
    }

}

