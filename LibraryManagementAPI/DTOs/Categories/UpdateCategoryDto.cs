using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.DTOs.Categories;

public sealed record UpdateCategoryDto([Required, MinLength(100)] string Name);
