namespace LibraryManagementAPI.DTOs.Authors;

public sealed record AuthorUpdateDto(string FirstName, string LastName, int? YearOfBirth);
