using CryptoDoge.BLL.Dtos;
using FluentValidation;

namespace CryptoDoge.BLL.ValidationDtos
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
	{
		public LoginDtoValidator()
		{
			RuleFor(ent => ent.EmailAddress)
				.NotEmpty().WithMessage("Email is required")
				.EmailAddress().WithMessage("Email must be an email address");
			RuleFor(ent => ent.Password)
				.MinimumLength(8).WithMessage("Password must be at least 8 character")
				.NotEmpty().WithMessage("Password is required");
		}
	}
}
