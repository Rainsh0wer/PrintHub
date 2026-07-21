using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PrintHub.Api.Common;

namespace PrintHub.Api.Filters;

/// <summary>
/// Runs any registered FluentValidation validator for each action argument
/// before the action executes, short-circuiting with 400 on failure. Field-level
/// validation therefore stays out of both controllers and services; business
/// rules remain in the service layer.
/// </summary>
public class ValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _services;

    public ValidationFilter(IServiceProvider services) => _services = services;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null) continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            if (_services.GetService(validatorType) is not IValidator validator) continue;

            var validationContext = new ValidationContext<object>(argument);
            var result = await validator.ValidateAsync(validationContext);
            if (result.IsValid) continue;

            var errors = result.Errors.Select(e => e.ErrorMessage).ToArray();
            context.Result = new BadRequestObjectResult(ApiResponse.Fail("Validation failed.", errors));
            return;
        }

        await next();
    }
}
