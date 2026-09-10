using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using WebApp1.Api.Configuration;

namespace WebApp1.Api.Services;

public class ExternalLoginExceptionFilter(
    IOptions<FrontendOptions> frontendOptions,
    ILogger<ExternalLoginExceptionFilter> logger) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        logger.LogError(context.Exception, "External login failed");
        context.Result = new RedirectResult($"{frontendOptions.Value.Url}/login?error=unexpected");
        context.ExceptionHandled = true;
    }
}