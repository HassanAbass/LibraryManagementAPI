using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.DTOs.Categories;

public sealed record CategoryUpdateDto([Required, MinLength(100)] string Name);
