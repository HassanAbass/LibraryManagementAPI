using FluentValidation;
using LibraryManagementAPI.DTOs.Categories;

namespace LibraryManagementAPI.Validators.Category
{

    public class UpdateCategoryValidator : AbstractValidator<CategoryUpdateDto>
    {
        public UpdateCategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters");
        }
    }
}
