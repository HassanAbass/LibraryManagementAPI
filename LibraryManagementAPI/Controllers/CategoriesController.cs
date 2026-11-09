using LibraryManagementAPI.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers;

public static class CategoriesController
{
    private static List<Category> _categories = new List<Category>([
        new Category { Id = 1, Name = "test" },
        new Category { Id = 2, Name = "AC" },
        new Category { Id = 3, Name = "Fiber" },
        ]);

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

    }

    private static async Task<Results<NoContent, ProblemHttpResult>> Delete(int id)
    {
        var categoryToBeDeleted = _categories.FirstOrDefault(c => c.Id == id);

        if(categoryToBeDeleted is null)
        {
            return TypedResults.Problem(statusCode: StatusCodes.Status404NotFound);
        }
        _categories.Remove(categoryToBeDeleted);
        return TypedResults.NoContent();
    }


    private static async Task<Results<ProblemHttpResult, CreatedAtRoute<Category>>> Create(Category category)
    {
        if (string.IsNullOrWhiteSpace(category.Name))
        {
            return TypedResults.Problem(
                detail: "Name not correct",
                statusCode: StatusCodes.Status400BadRequest);
        }
        if (_categories.Any(cat => cat.Id == category.Id) || _categories.Any(cat => cat.Name == category.Name))
        {
            return TypedResults.Problem(
                detail: "Resource already exists",
                statusCode: StatusCodes.Status409Conflict);
        }
        _categories.Add(category);

        return TypedResults.CreatedAtRoute(category, "GetById", new { id = category.Id });
    }

    private static async Task<Ok<List<Category>>> GetAll()
    {
        return TypedResults.Ok(_categories);
    }
    private static async Task<Results<Ok<Category>, ProblemHttpResult>> GetById(int id)
    {
        var category = _categories.FirstOrDefault(cat => cat.Id == id);
        if (category is null)
        {
            return TypedResults.Problem(detail: "Enter valid ID", statusCode: StatusCodes.Status404NotFound);
        }
        return TypedResults.Ok(category);
    }
}
