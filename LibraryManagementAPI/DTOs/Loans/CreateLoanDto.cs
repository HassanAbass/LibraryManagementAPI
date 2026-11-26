namespace LibraryManagementAPI.DTOs.Loans;

public sealed record CreateLoanDto(int MemberId, int BookId, DateTime DueDate);
