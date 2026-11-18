using Microsoft.AspNetCore.Mvc.Filters;

namespace LibraryManagementAPI.Filter
{
    public class LogActionFilter (ILogger<LogActionFilter> _logger) : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            _logger.LogInformation("Executing action: {ActionName}", context.ActionDescriptor.DisplayName);
            
            foreach(var argument in context.ActionArguments)
            {
                _logger.LogInformation("Argument: {ArgumentName} = {ArgumentValue}", argument.Key, argument.Value);
            }

            Console.ForegroundColor = ConsoleColor.White;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            _logger.LogInformation("Executed action: {ActionName}, status code {StatusCode}", context.ActionDescriptor.DisplayName, context.HttpContext.Response.StatusCode);
            Console.ForegroundColor = ConsoleColor.White;

        }
    }
}
