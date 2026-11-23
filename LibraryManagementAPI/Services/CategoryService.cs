using ErrorOr;
using FluentValidation;
using LibraryManagementAPI.DataAccess;
using LibraryManagementAPI.DTOs.Categories;
using LibraryManagementAPI.Errors;
using LibraryManagementAPI.Mapping;
using LibraryManagementAPI.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Services;

public sealed class CategoryService(
    LibraryDbContext dbContext,
    CategoryMapper categoryMapper,
    IValidator<CreateCategoryDto> createCategoryValidator,
    IValidator<UpdateCategoryDto> updateCategoryValidator) : ICategoryService
{
    public async Task<ErrorOr<List<ReadCategoryDto>>> GetAllAsync()
    {
        var categories = await dbContext.Categories.AsNoTracking().ToListAsync();
        return categories.Select(categoryMapper.ToReadDto).ToList();
    }

    public async Task<ErrorOr<ReadCategoryDto>> GetByIdAsync(int id)
    {
        var category = await dbContext.Categories.FindAsync(id);
        if (category is null)
        {
            return CategoryErrors.NotFound(id);
        }

        return categoryMapper.ToReadDto(category);
    }

    public async Task<ErrorOr<ReadCategoryDto>> CreateAsync(CreateCategoryDto request)
    {
        var validationResult = await createCategoryValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors
                .Select(error => Error.Validation(code: error.PropertyName, description: error.ErrorMessage))
                .ToList();
        }

        if (await dbContext.Categories.AnyAsync(category => category.Name == request.Name))
        {
            return CategoryErrors.Duplicate;
        }

        var category = categoryMapper.ToEntity(request);
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();
        return categoryMapper.ToReadDto(category);
    }

    public async Task<ErrorOr<ReadCategoryDto>> UpdateAsync(int id, UpdateCategoryDto request)
    {
        var validationResult = await updateCategoryValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors
                .Select(error => Error.Validation(code: error.PropertyName, description: error.ErrorMessage))
                .ToList();
        }

        var category = await dbContext.Categories.FindAsync(id);
        if (category is null)
        {
            return CategoryErrors.NotFound(id);
        }

        category.Name = request.Name;
        await dbContext.SaveChangesAsync();
        return categoryMapper.ToReadDto(category);
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(int id)
    {
        var category = await dbContext.Categories.FindAsync(id);
        if (category is null)
        {
            return CategoryErrors.NotFound(id);
        }

        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync();
        return Result.Deleted;
    }
}
