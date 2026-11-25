using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.DTOs.Authors;

public sealed record PatchAuthorDto()
{
    [MinLength(1)]
    public string? FirstName { get; init; }

    [MinLength(1)]
    [MaxLength(50)]
    public string? LastName { get; init; }

    [Range(-2000, 2026)]
    public int? YearOfBirth { get; init; }
}
