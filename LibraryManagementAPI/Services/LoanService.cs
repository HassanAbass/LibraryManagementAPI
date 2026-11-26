using ErrorOr;
using FluentValidation;
using LibraryManagementAPI.DataAccess;
using LibraryManagementAPI.DTOs.Loans;
using LibraryManagementAPI.Errors;
using LibraryManagementAPI.Mapping;
using LibraryManagementAPI.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Services;

public sealed class LoanService(
    LibraryDbContext dbContext,
    LoanMapper loanMapper,
    IValidator<CreateLoanDto> createLoanValidator,
    IValidator<UpdateLoanDto> updateLoanValidator) : ILoanService
{
    public async Task<ErrorOr<IEnumerable<ReadLoanDto>>> GetAllAsync()
    {
        var loans = await dbContext.Loans.ToListAsync();
        return loans.Select(loanMapper.ToReadDto).ToList();
    }

    public async Task<ErrorOr<ReadLoanDto>> GetByIdAsync(int id)
    {
        var loan = await dbContext.Loans.FirstOrDefaultAsync(loan => loan.Id == id);
        return loan is null ? LoanErrors.NotFound(id) : loanMapper.ToReadDto(loan);
    }

    public async Task<ErrorOr<ReadLoanDto>> CreateAsync(CreateLoanDto request)
    {
        var validationResult = await createLoanValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors
                .Select(error => Error.Validation(code: error.PropertyName, description: error.ErrorMessage))
                .ToList();
        }

        var member = await dbContext.Members.FirstOrDefaultAsync(member => member.Id == request.MemberId);
        if (member is null)
        {
            return LoanErrors.MemberNotFound(request.MemberId);
        }

        if (!member.IsActive)
        {
            return LoanErrors.MemberInactive(request.MemberId);
        }

        var book = await dbContext.Books.FirstOrDefaultAsync(book => book.Id == request.BookId);
        if (book is null)
        {
            return LoanErrors.BookNotFound(request.BookId);
        }

        if (book.AvailableCopies <= 0)
        {
            return LoanErrors.NoCopiesAvailable(request.BookId);
        }

        var loan = loanMapper.ToEntity(request);
        loan.LoanDate = DateTime.UtcNow;
        book.AvailableCopies--;
        dbContext.Loans.Add(loan);
        await dbContext.SaveChangesAsync();
        return loanMapper.ToReadDto(loan);
    }

    public async Task<ErrorOr<ReadLoanDto>> UpdateAsync(int id, UpdateLoanDto request)
    {
        var loan = await dbContext.Loans.FirstOrDefaultAsync(loan => loan.Id == id);
        if (loan is null)
        {
            return LoanErrors.NotFound(id);
        }

        if (loan.ReturnedAt is not null)
        {
            return LoanErrors.AlreadyReturned(id);
        }

        var validationResult = await updateLoanValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors
                .Select(error => Error.Validation(code: error.PropertyName, description: error.ErrorMessage))
                .ToList();
        }

        loanMapper.UpdateEntity(request, loan);
        await dbContext.SaveChangesAsync();
        return loanMapper.ToReadDto(loan);
    }

    public async Task<ErrorOr<ReadLoanDto>> ReturnAsync(int id)
    {
        var loan = await dbContext.Loans
            .Include(loan => loan.Book)
            .FirstOrDefaultAsync(loan => loan.Id == id);
        if (loan is null)
        {
            return LoanErrors.NotFound(id);
        }

        if (loan.ReturnedAt is not null)
        {
            return LoanErrors.AlreadyReturned(id);
        }

        loan.ReturnedAt = DateTime.UtcNow;
        loan.Book.AvailableCopies++;
        await dbContext.SaveChangesAsync();
        return loanMapper.ToReadDto(loan);
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(int id)
    {
        var loan = await dbContext.Loans
            .Include(loan => loan.Book)
            .FirstOrDefaultAsync(loan => loan.Id == id);
        if (loan is null)
        {
            return LoanErrors.NotFound(id);
        }

        if (loan.ReturnedAt is null)
        {
            loan.Book.AvailableCopies++;
        }

        dbContext.Loans.Remove(loan);
        await dbContext.SaveChangesAsync();
        return Result.Deleted;
    }
}
