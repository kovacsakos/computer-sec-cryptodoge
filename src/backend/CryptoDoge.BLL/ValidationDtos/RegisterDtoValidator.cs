using CryptoDoge.BLL.Dtos;
using FluentValidation;

namespace CryptoDoge.BLL.ValidationDtos
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
	{
		public RegisterDtoValidator()
		{
			RuleFor(ent => ent.EmailAddress)
				.NotEmpty().WithMessage("Email is required")
				.EmailAddress().WithMessage("Email must be an email address");
			RuleFor(ent => ent.UserName)
				.NotEmpty().WithMessage("Username is required");
			RuleFor(ent => ent.Password)
				.MinimumLength(8).WithMessage("Password must be at least 8 character")
				.NotEmpty().WithMessage("Password is required")
				.Matches("[A-Z]").WithMessage("Password must contain one or more capital letters.")
				.Matches("[a-z]").WithMessage("Password must contain one or more lowercase letters.")
				.Matches(@"\d").WithMessage("Password must contain one or more digits.")
				.Matches(@"[][""!@$%^&*(){}:;<>,.?/+_=|'~\\-]").WithMessage("Password must contain one or more special characters.");
		}
	}
}
