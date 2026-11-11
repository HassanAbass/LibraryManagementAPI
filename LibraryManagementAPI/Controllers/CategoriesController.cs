using LibraryManagementAPI.DataAccess;
using LibraryManagementAPI.Entities;
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
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapGet("/{id:int}", GetById)
            .WithName("GetById")
            .Produces<Category>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapPut("/{id:int}", Update)
            .Produces<Category>(StatusCodes.Status200OK)
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

    private static async Task<Results<ProblemHttpResult, CreatedAtRoute<Category>>> Create(Category category, LibraryDbContext dbContext)
    {
        if (string.IsNullOrWhiteSpace(category.Name))
        {
            return TypedResults.Problem(
                detail: "Name not correct",
                statusCode: StatusCodes.Status400BadRequest);
        }
        if (await dbContext.Categories.AnyAsync(existing => existing.Name == category.Name))
        {
            return TypedResults.Problem(
                detail: "Resource already exists",
                statusCode: StatusCodes.Status409Conflict);
        }

        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();
        return TypedResults.CreatedAtRoute(category, "GetById", new { id = category.Id });
    }

    private static async Task<Ok<List<Category>>> GetAll(LibraryDbContext dbContext)
    {
        return TypedResults.Ok(await dbContext.Categories.AsNoTracking().ToListAsync());
    }

    private static async Task<Results<Ok<Category>, ProblemHttpResult>> GetById(int id, LibraryDbContext dbContext)
    {
        var category = await dbContext.Categories.FindAsync(id);
        if (category is null)
        {
            return TypedResults.Problem(detail: "Enter valid ID", statusCode: StatusCodes.Status404NotFound);
        }
        return TypedResults.Ok(category);
    }

    private static async Task<Results<Ok<Category>, ProblemHttpResult>> Update(int id, Category request, LibraryDbContext dbContext)
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
        return TypedResults.Ok(category);
    }

}
