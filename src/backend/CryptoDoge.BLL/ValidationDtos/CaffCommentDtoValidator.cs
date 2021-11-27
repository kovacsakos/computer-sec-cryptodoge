using CryptoDoge.BLL.Dtos;
using FluentValidation;

namespace CryptoDoge.BLL.ValidationDtos
{
    public class CaffCommentDtoValidator: AbstractValidator<CaffCommentDto>
    {
        public CaffCommentDtoValidator()
        {
            RuleFor(ent => ent.Comment)
                .NotEmpty().WithMessage("Comment is required");
        }
    }
}
