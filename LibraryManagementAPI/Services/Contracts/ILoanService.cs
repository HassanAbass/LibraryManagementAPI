using ErrorOr;
using LibraryManagementAPI.DTOs.Loans;

namespace LibraryManagementAPI.Services.Contracts;

public interface ILoanService
{
    Task<ErrorOr<IEnumerable<ReadLoanDto>>> GetAllAsync();
    Task<ErrorOr<ReadLoanDto>> GetByIdAsync(int id);
    Task<ErrorOr<ReadLoanDto>> CreateAsync(CreateLoanDto request);
    Task<ErrorOr<ReadLoanDto>> UpdateAsync(int id, UpdateLoanDto request);
    Task<ErrorOr<ReadLoanDto>> ReturnAsync(int id);
    Task<ErrorOr<Deleted>> DeleteAsync(int id);
}
