using CryptoDoge.BLL.Dtos;
using FluentValidation;
using System;
using System.Linq;

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
				.NotEmpty().WithMessage("Password is required")
				.Matches("[A-Z]").WithMessage("Password must contain one or more capital letters.")
				.Matches("[a-z]").WithMessage("Password must contain one or more lowercase letters.")
				.Matches(@"\d").WithMessage("Password must contain one or more digits.")
				.Must(ContainsNonAlphaNumeric).WithMessage("Password must contain one or more special characters.");
		}

        private bool ContainsNonAlphaNumeric(string arg)
        {
            return arg.Any(c => !char.IsLetterOrDigit(c));
        }
    }
}
