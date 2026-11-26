using ErrorOr;

namespace LibraryManagementAPI.Errors;

public static class LoanErrors
{
    public static Error NotFound(int id) =>
        Error.NotFound(description: $"Loan with id {id} not found.");

    public static Error MemberNotFound(int id) =>
        Error.NotFound(description: $"Member with id {id} not found.");

    public static Error MemberInactive(int id) =>
        Error.Validation(description: $"Member with id {id} is inactive.");

    public static Error BookNotFound(int id) =>
        Error.NotFound(description: $"Book with id {id} not found.");

    public static Error NoCopiesAvailable(int id) =>
        Error.Conflict(description: $"Book with id {id} has no available copies.");

    public static Error AlreadyReturned(int id) =>
        Error.Conflict(description: $"Loan with id {id} has already been returned.");
}
