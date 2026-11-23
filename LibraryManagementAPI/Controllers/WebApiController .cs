using ErrorOr;
using LibraryManagementAPI.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers;

public class WebApiController : ControllerBase
{
    protected ActionResult Problem(IReadOnlyList<Error> errors)
    {
        var problemDetails = errors.ToProblemDetails();
        return new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status,
            ContentTypes = { "application/problem+json", "application/problem+xml" }
        };
    }
}
