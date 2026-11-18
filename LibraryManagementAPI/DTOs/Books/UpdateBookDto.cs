namespace LibraryManagementAPI.DTOs.Books;

public class UpdateBookDto
{
    public required string Title { get; set; }
    public required int TotalCopies { get; set; }
    public required int CategoryId { get; set; }
    public ICollection<int> AuthorIds { get; set; } = [];

}
