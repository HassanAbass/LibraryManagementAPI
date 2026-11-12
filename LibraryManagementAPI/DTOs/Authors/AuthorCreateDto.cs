using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.DTOs.Authors;

public sealed record AuthorCreateDto(string FirstName, string LastName, int? YearOfBirth);
