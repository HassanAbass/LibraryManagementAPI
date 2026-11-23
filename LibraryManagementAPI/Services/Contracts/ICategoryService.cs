using ErrorOr;
using LibraryManagementAPI.DTOs.Categories;

namespace LibraryManagementAPI.Services.Contracts;

public interface ICategoryService
{
    Task<ErrorOr<List<ReadCategoryDto>>> GetAllAsync();
    Task<ErrorOr<ReadCategoryDto>> GetByIdAsync(int id);
    Task<ErrorOr<ReadCategoryDto>> CreateAsync(CreateCategoryDto request);
    Task<ErrorOr<ReadCategoryDto>> UpdateAsync(int id, UpdateCategoryDto request);
    Task<ErrorOr<Deleted>> DeleteAsync(int id);
}
