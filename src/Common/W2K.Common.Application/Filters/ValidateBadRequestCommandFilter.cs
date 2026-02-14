using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DFI.Common.Application.Filters;

// Suppress Sonar warning: S2325 - Methods and properties that don't access instance data should be static
#pragma warning disable S2325

public class ValidateBadRequestCommandFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        //Getting currency called method type so can stop checking this for others type except post and put.
        //Because we will pass command in post and put only.
        var methodType = context.HttpContext.Request.Method;

        //Checking for command in actionDescriptor first.
        //POST and PUT is hardcoded because these are method type and will be same always.
        if (context.ActionDescriptor.Parameters.Any(x => x.Name == "command") && (methodType == "POST" || methodType == "PUT"))
        {
            //Checking for command and its hardcoded and will use command in controller only.
            var param = context.ActionArguments.SingleOrDefault(x => x.Key == "command");
            if (param.Value is null)
            {
                context.Result = new BadRequestObjectResult("We were unable to process your payload. Please check the payload"
                    + " you are sending and confirm that each field conforms to their respective data type constraints as specified in the schema.");
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        //To Do
    }
}

#pragma warning restore S2325
