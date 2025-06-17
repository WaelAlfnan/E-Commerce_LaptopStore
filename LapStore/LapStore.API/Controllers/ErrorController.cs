using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LapStore.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ErrorController : ControllerBase
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Handles unhandled exceptions and returns a ProblemDetails response.
        /// </summary>
        [Route("/Error")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult HandleError()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context?.Error;

            if (exception != null)
            {
                _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);
            }

            // Return a generic problem details response
            return Problem(
                detail: "An unexpected error occurred. Please try again later.",
                statusCode: 500,
                title: "Internal Server Error"
            );
        }
    }
} 