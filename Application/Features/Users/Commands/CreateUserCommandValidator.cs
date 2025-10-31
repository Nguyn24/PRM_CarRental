using FluentValidation;

namespace Application.Features.Users.Commands;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MaximumLength(100).WithMessage("Full name must not exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be a valid email address");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .Must(p => !string.IsNullOrWhiteSpace(p))
            .WithMessage("Password must not be whitespace only");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("Invalid role");
    }
}
