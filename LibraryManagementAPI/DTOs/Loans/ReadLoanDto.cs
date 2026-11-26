using LibraryManagementAPI.Enums;

namespace LibraryManagementAPI.DTOs.Loans;

public sealed record ReadLoanDto(
    int Id,
    int MemberId,
    int BookId,
    DateTime LoanDate,
    DateTime DueDate,
    DateTime? ReturnedAt,
    LoanStatus Status);
