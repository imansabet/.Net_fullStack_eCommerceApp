using eCommerceApp.Application.DTOs.Identity;
using FluentValidation;

namespace eCommerceApp.Application.Validations.Authentication;

public class LoginUserValidator : AbstractValidator<LoginUser>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.Email)
           .NotEmpty().WithMessage("Email Is Required .")
           .EmailAddress().WithMessage("Invalid Email Format .");

        RuleFor(x => x.Password)
           .NotEmpty().WithMessage("Password  Is Required .");
    }
}
