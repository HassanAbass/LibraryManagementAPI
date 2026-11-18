using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.DTOs.Authors;

public sealed record CreateAuthorDto(string FirstName, string LastName, int? YearOfBirth);
