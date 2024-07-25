using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace StallosDotnetPleno.Api.Filter;

public class JsonExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Result is ObjectResult objectResult)
        {
            if (objectResult.Value is ValidationProblemDetails validationProblem)
            {
                if (validationProblem.Errors.ContainsKey("postPessoaView"))
                {
                    validationProblem.Errors["Details"] = validationProblem.Errors["postPessoaView"];
                    validationProblem.Errors.Remove("postPessoaView");
                }

                context.Result = new ObjectResult(validationProblem)
                {
                    StatusCode = 400
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
