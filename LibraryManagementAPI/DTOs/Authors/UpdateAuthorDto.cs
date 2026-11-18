namespace LibraryManagementAPI.DTOs.Authors;

public sealed record UpdateAuthorDto(string FirstName, string LastName, int? YearOfBirth);
