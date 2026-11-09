using LibraryManagementAPI.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

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
        group.MapPost("/", Create);
        group.MapGet("/{id:int}", GetById)
            .WithName("GetById")
            .Produces<Category>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

    }

    private static async Task<Results<Conflict, BadRequest, CreatedAtRoute<Category>>> Create(Category category)
    {
        if (string.IsNullOrWhiteSpace(category.Name))
        {
            return TypedResults.BadRequest();
        }
        if (_categories.Any(cat => cat.Id == category.Id) || _categories.Any(cat => cat.Name == category.Name))
        {
            return TypedResults.Conflict();
        }
        _categories.Add(category);

        return TypedResults.CreatedAtRoute(category, "GetById", new { id = category.Id });
    }

    private static async Task<Ok<List<Category>>> GetAll()
    {
        return TypedResults.Ok(_categories);
    }
    private static async Task<Results<Ok<Category>, NotFound>> GetById(int id)
    {
        var category = _categories.FirstOrDefault(cat => cat.Id == id);
        if (category is null)
        {
            return TypedResults.NotFound();
        }
        return TypedResults.Ok(category);
    }
}
