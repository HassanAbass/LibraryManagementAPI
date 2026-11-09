using LibraryManagementAPI.Entities;

namespace LibraryManagementAPI.Controllers;

public static class CategoriesController
{

    public static void MapCategoryEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var group = endpointRouteBuilder.MapGroup("/categories").WithTags("Categories");

        group.Map("/", GetAll);
    }
    private static async Task<IResult> GetAll()
    {
        var categories = new List<Category>([new Category { Id = 1, Name = "test" }]);
        return Results.Ok(categories);
    }
}
