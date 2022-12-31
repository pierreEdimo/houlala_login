using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using user_service.exception;

namespace user_service.filter;

public class HttpResponseExceptionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is LoginException exception)
        {
            HttpExceptionResult resultObject = new HttpExceptionResult()
            {
                Status = exception.Status,
                Message = exception.Message,
                Source = exception.Source,
            };

            context.Result = new ObjectResult(resultObject)
            {
                StatusCode = exception.Status
            };

            context.ExceptionHandled = true;
        }
    }
}