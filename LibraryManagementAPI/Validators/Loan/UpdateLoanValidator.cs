using FluentValidation;
using LibraryManagementAPI.DTOs.Loans;

namespace LibraryManagementAPI.Validators.Loan;

public sealed class UpdateLoanValidator : AbstractValidator<UpdateLoanDto>
{
    public UpdateLoanValidator()
    {
        RuleFor(x => x.DueDate).Must(dueDate => dueDate > DateTime.UtcNow)
            .WithMessage("Due date must be in the future");
    }
}
