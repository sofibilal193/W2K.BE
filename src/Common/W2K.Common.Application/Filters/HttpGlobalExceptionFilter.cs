using System.Diagnostics;
using System.Net;
using DFI.Common.Application.Validations;
using DFI.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DFI.Common.Application.Filters;

public class HttpGlobalExceptionFilter(IOptions<ApiBehaviorOptions> options, ILogger<HttpGlobalExceptionFilter> logger) : IExceptionFilter
{
    private readonly ApiBehaviorOptions _options = options.Value;
    private readonly ILogger<HttpGlobalExceptionFilter> _logger = logger;

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(
            new EventId(context.Exception.HResult),
            context.Exception,
            "");

        switch (context.Exception)
        {
            case NotFoundException:
                context.Result = new NotFoundResult();
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                break;
            case ArgumentException:
                context.Result = new BadRequestObjectResult("We were unable to process your payload. Please check the payload you are sending"
                    + " and confirm that each field conforms to their respective data type constraints as specified in the schema.");
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;
            case ValidationException exception:
                var details = GetApiValidationDetails(context, exception);
                details.TraceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
                context.Result = new BadRequestObjectResult(details);
                context.ExceptionHandled = true;
                break;
            case HttpRequestException reqException:
                if (reqException.StatusCode == HttpStatusCode.Forbidden)
                {
                    context.Result = new ForbidResult();
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                }
                break;
            default:
                context.Result = null;
                context.ExceptionHandled = false;
                break;
        }
    }

    private ApiValidationProblemDetails GetApiValidationDetails(ActionContext context, ValidationException exception)
    {
        foreach (var error in exception.Errors)
        {
            context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
        var details = new ApiValidationProblemDetails(context.ModelState);
        if (_options.ClientErrorMapping.TryGetValue((int)HttpStatusCode.BadRequest, out var data))
        {
            details.Title ??= data.Title;
            details.Type ??= data.Link;
        }
        foreach (var property in exception.Errors.Select(x => x.PropertyName).Distinct())
        {
            details.ErrorCodes.Add(property, [.. exception.Errors.Where(x => x.PropertyName == property).Select(x => x.ErrorCode)]);
        }

        return details;
    }
}
