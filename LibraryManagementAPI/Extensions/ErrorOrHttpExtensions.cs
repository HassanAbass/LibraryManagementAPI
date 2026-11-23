using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Extensions;

public static class ErrorOrHttpExtensions
{
    public static ProblemDetails ToProblemDetails(this IReadOnlyList<Error> errors)
    {
        var statusCode = GetStatusCode(errors[0].Type);
        if (errors.All(error => error.Type == ErrorType.Validation))
        {
            var validationErrors = errors
                .GroupBy(error => string.IsNullOrWhiteSpace(error.Code) ? "Request" : error.Code)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.Description).ToArray());

            return new ValidationProblemDetails(validationErrors)
            {
                Status = statusCode
            };
        }

        return new ProblemDetails
        {
            Detail = string.Join(" ", errors.Select(error => error.Description)),
            Status = statusCode
        };
    }

    private static int GetStatusCode(ErrorType errorType) => errorType switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        ErrorType.Failure or ErrorType.Unexpected => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status500InternalServerError
    };
}
