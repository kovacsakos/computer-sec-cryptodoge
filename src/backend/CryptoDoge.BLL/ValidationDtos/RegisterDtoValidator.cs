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
				.NotEmpty().WithMessage("Password is required");
		}
	}
}
