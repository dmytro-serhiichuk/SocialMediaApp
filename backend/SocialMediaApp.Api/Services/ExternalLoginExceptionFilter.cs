using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using SocialMediaApp.Api.Configuration;

namespace SocialMediaApp.Api.Services;

public class ExternalLoginExceptionFilter(
    string errorRedirectPath,
    IOptions<FrontendOptions> frontendOptions,
    ILogger<ExternalLoginExceptionFilter> logger) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        logger.LogError(context.Exception, "External login failed");
        context.Result = new RedirectResult($"{frontendOptions.Value.Url}{errorRedirectPath}?error=unexpected");
        context.ExceptionHandled = true;
    }
}