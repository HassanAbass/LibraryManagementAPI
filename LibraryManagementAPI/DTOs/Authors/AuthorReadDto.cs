namespace LibraryManagementAPI.DTOs.Authors;

public sealed record AuthorReadDto(int Id, string FirstName, string LastName, int? YearOfBirth);
