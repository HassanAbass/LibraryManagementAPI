using ErrorOr;

namespace LibraryManagementAPI.Errors;

public static class MembersError
{
    public static Error NotFound(int id) =>
        Error.NotFound(description: $"Member with id {id} not found.");

    public static Error Duplicate =>
        Error.Conflict(description: "A member with this first and last name already exists.");

    public static Error NamesRequired =>
        Error.Validation(code: "NamesRequired", description: "First name and last name are required.");
}
