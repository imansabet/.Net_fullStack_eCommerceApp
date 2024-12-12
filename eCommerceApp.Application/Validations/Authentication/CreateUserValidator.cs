using eCommerceApp.Application.DTOs.Identity;
using FluentValidation;

namespace eCommerceApp.Application.Validations.Authentication;

public class CreateUserValidator :AbstractValidator<CreateUser>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("FullName Is Required .");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("FullName Is Required .")
            .EmailAddress().WithMessage("Invalid Email Format .");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password  Is Required .")
            .MinimumLength(8).WithMessage("Password must be at least 8 Characters Long .")
            .Matches(@"[A-Z]").WithMessage("Password must Contain at least one UpperCase Letter .")
            .Matches(@"[a-z]").WithMessage("Password must Contain at least one LowerCase Letter .")
            .Matches(@"\d").WithMessage("Password must Contain at least one number .")
            .Matches(@"[^\w]").WithMessage("Password must Contain at least one special character .");

        RuleFor(x => x.ConfirmPassword)
           .Equal(x => x.Password).WithMessage("Password do not match.");
    }
}
