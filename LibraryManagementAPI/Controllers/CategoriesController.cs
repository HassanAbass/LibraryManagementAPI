using LibraryManagementAPI.DTOs.Categories;
using LibraryManagementAPI.Extensions;
using LibraryManagementAPI.Services.Contracts;

namespace LibraryManagementAPI.Controllers;

public static class CategoriesController
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var group = endpointRouteBuilder.MapGroup("/api/categories").WithTags("Categories");

        group.MapGet("/", GetAll)
            .Produces<List<ReadCategoryDto>>(StatusCodes.Status200OK);
        group.MapDelete("/{id:int}", Delete)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapPost("/", Create)
            .Produces<ReadCategoryDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict);
        group.MapGet("/{id:int}", GetById)
            .WithName("GetById")
            .Produces<ReadCategoryDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapPut("/{id:int}", Update)
            .Produces<ReadCategoryDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> GetAll(ICategoryService categoryService)
    {
        var result = await categoryService.GetAllAsync();
        return result.IsError
            ? TypedResults.Problem(result.Errors.ToProblemDetails())
            : TypedResults.Ok(result.Value);
    }

    private static async Task<IResult> GetById(int id, ICategoryService categoryService)
    {
        var result = await categoryService.GetByIdAsync(id);
        return result.IsError
            ? TypedResults.Problem(result.Errors.ToProblemDetails())
            : TypedResults.Ok(result.Value);
    }

    private static async Task<IResult> Create(CreateCategoryDto request, ICategoryService categoryService)
    {
        var result = await categoryService.CreateAsync(request);
        return result.IsError
            ? TypedResults.Problem(result.Errors.ToProblemDetails())
            : TypedResults.CreatedAtRoute(result.Value, "GetById", new { id = result.Value.Id });
    }

    private static async Task<IResult> Update(int id, UpdateCategoryDto request, ICategoryService categoryService)
    {
        var result = await categoryService.UpdateAsync(id, request);
        return result.IsError
            ? TypedResults.Problem(result.Errors.ToProblemDetails())
            : TypedResults.Ok(result.Value);
    }

    private static async Task<IResult> Delete(int id, ICategoryService categoryService)
    {
        var result = await categoryService.DeleteAsync(id);
        return result.IsError
            ? TypedResults.Problem(result.Errors.ToProblemDetails())
            : TypedResults.NoContent();
    }
}
