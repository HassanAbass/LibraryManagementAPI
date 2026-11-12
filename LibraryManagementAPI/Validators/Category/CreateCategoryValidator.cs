using FluentValidation;
using LibraryManagementAPI.DTOs.Categories;

namespace LibraryManagementAPI.Validators.Category
{

    public class CreateCategoryValidator : AbstractValidator<CategoryCreateDto>
    {
        public CreateCategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters");
        }
    }
}
