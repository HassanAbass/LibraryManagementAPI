using ErrorOr;

namespace LibraryManagementAPI.Errors;

public static class CategoryErrors
{
    public static Error NotFound(int id) =>
        Error.NotFound(description: $"Category with id {id} not found.");

    public static Error Duplicate =>
        Error.Conflict(description: "A category with this name already exists.");
}
