using CryptoDoge.BLL.Dtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
				.MinimumLength(4).WithMessage("Password must be at least 4 character")
				.NotEmpty().WithMessage("Password is required");
		}
	}
}
