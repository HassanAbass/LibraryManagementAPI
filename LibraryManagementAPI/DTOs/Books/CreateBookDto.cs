using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.DTOs.Books;

public class CreateBookDto
{
    [Required]
    public required string Title { get; set; }
    [Range(1, int.MaxValue)]
    [Required]
    public required int TotalCopies { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be a positive integer.")]
    [Required]
    public required int CategoryId { get; set; }
    [MinLength(1, ErrorMessage = "At least one author must be selected.")]
    public ICollection<int> AuthorIds { get; set; } = [];
}
