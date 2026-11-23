namespace LibraryManagementAPI.Services;

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int TotalPages);
