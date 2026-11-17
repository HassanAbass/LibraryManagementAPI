using FluentValidation;

namespace LibraryManagementAPI.Filter
{
    public class ValidationFilter<T> : IEndpointFilter where T : class
    {
        public async  ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var validator = context.HttpContext.RequestServices.GetRequiredService<IValidator<T>>();
            var dto = context.Arguments.OfType<T>().FirstOrDefault();

            var validationResult = await validator.ValidateAsync(dto);

            if(!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            return await next(context);
        }
    }
}
