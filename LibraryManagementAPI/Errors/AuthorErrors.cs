using ErrorOr;

namespace LibraryManagementAPI.Errors;

public static class AuthorErrors
{
    public static Error NotFound(int id) =>
        Error.NotFound(description: $"Author with id {id} not found.");

    public static Error Duplicate =>
        Error.Conflict(description: "An author with this first and last name already exists.");

    public static Error NamesRequired =>
        Error.Validation(code: "NamesRequired", description: "First name and last name are required.");
}
