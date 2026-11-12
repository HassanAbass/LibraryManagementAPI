using FluentValidation;
using LibraryManagementAPI.DataAccess;
using LibraryManagementAPI.DTOs.Categories;
using LibraryManagementAPI.Extensions;
using LibraryManagementAPI.Mapping;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Controllers;

public static class CategoriesController
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var group = endpointRouteBuilder.MapGroup("/api/categories").WithTags("Categories");

        group.MapGet("/", GetAll);
        group.MapDelete("/{id:int}", Delete)
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapPost("/", Create)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict);
        group.MapGet("/{id:int}", GetById)
            .WithName("GetById")
            .Produces<CategoryReadDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapPut("/{id:int}", Update)
            .Produces<CategoryReadDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> Delete(int id, LibraryDbContext dbContext)
    {
        var category = await dbContext.Categories.FindAsync(id);
        if (category is null)
        {
            return TypedResults.Problem(statusCode: StatusCodes.Status404NotFound);
        }

        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    private static async Task<Results<ProblemHttpResult, CreatedAtRoute<CategoryReadDto>>> Create(CategoryCreateDto request, LibraryDbContext dbContext, CategoryMapper categoryMapper, IValidator<CategoryCreateDto> validator)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return validationResult.ToProblem();
        }
        if (await dbContext.Categories.AnyAsync(existing => existing.Name == request.Name))
        {
            return validationResult.ToProblem();
        }

        var category = categoryMapper.ToEntity(request);
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();
        var response = categoryMapper.ToReadDto(category);
        return TypedResults.CreatedAtRoute(response, "GetById", new { id = category.Id });
    }

    private static async Task<Ok<List<CategoryReadDto>>> GetAll(LibraryDbContext dbContext, CategoryMapper categoryMapper)
    {
        var categories = await dbContext.Categories.AsNoTracking().ToListAsync();
        return TypedResults.Ok(categories.Select(categoryMapper.ToReadDto).ToList());
    }

    private static async Task<Results<Ok<CategoryReadDto>, ProblemHttpResult>> GetById(int id, LibraryDbContext dbContext, CategoryMapper categoryMapper)
    {
        var category = await dbContext.Categories.FindAsync(id);
        if (category is null)
        {
            return TypedResults.Problem(detail: "Enter valid ID", statusCode: StatusCodes.Status404NotFound);
        }
        return TypedResults.Ok(categoryMapper.ToReadDto(category));
    }

    private static async Task<Results<Ok<CategoryReadDto>, ProblemHttpResult>> Update(int id, CategoryUpdateDto request, LibraryDbContext dbContext, CategoryMapper categoryMapper)
    {
        var category = await dbContext.Categories.FindAsync(id);
        if (category is null)
        {
            return TypedResults.Problem(detail: "Enter valid ID", statusCode: StatusCodes.Status404NotFound);
        }

        if (request.Name is not null && string.IsNullOrWhiteSpace(request.Name))
        {
            return TypedResults.Problem(
                detail: "Name not correct",
                statusCode: StatusCodes.Status400BadRequest);
        }

        category.Name = request.Name ?? category.Name;
        await dbContext.SaveChangesAsync();
        return TypedResults.Ok(categoryMapper.ToReadDto(category));
    }
}
