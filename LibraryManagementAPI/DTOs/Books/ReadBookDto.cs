
namespace LibraryManagementAPI.DTOs.Books;

public class ReadBookDto
{
    public int Id { get; init; }
    public required string Title { get; init; }
    public int AvailableCopies { get; init; }
    public required int TotalCopies { get; init; }
    public required int CategoryId { get; init; }
    public string CategoryName { get; init; } = null!;
    public ICollection<BookAuthorDto> Authors { get; init; } = [];
}
