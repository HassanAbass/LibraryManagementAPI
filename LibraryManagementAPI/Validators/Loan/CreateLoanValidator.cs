using FluentValidation;
using LibraryManagementAPI.DTOs.Loans;

namespace LibraryManagementAPI.Validators.Loan;

public sealed class CreateLoanValidator : AbstractValidator<CreateLoanDto>
{
    public CreateLoanValidator()
    {
        RuleFor(x => x.MemberId).GreaterThan(0);
        RuleFor(x => x.BookId).GreaterThan(0);
        RuleFor(x => x.DueDate).Must(dueDate => dueDate > DateTime.UtcNow)
            .WithMessage("Due date must be in the future");
    }
}
