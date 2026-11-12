using FluentValidation;
using LibraryManagementAPI.DTOs.Authors;

namespace LibraryManagementAPI.Validators.Author
{
    public class CreateAuthorValidator : AbstractValidator<AuthorCreateDto>
    {
        public CreateAuthorValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(50).WithMessage("First name must not exceed 50 characters");
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters");
            RuleFor(x => x.YearOfBirth)
                .ExclusiveBetween(1500, 2023).WithMessage("Year of birth must be between 1500 and 2023");
        }
    }
}
