using LibraryManagementAPI.DTOs.Books;

namespace LibraryManagementAPI.DTOs.Authors;

public sealed record ReadAuthorDto(int Id, string FirstName, string LastName, int? YearOfBirth, ICollection<AuthorBooksDto> Books);
