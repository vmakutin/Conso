using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Conso.API.Exceptions
{
    public class GlobalExceptionHandler
    {
        private readonly ILogger logger;

        public GlobalExceptionHandler(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Invoke(HttpContext context)
        {
            var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
            if (contextFeature is null)
            {
                return;
            }

            var exception = contextFeature.Error;
            if (exception is null)
            {
                return;
            }

            logger.LogCritical(exception, "GlobalException Handler");

            var errorCode = StatusCodes.Status500InternalServerError;
            object message;

            switch (exception)
            {
                default:
                    if (true)
                    {
                        message = JObject.Parse(JsonConvert.SerializeObject(exception));
                    }

                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = errorCode;

            await context.Response.WriteAsync(JsonConvert.SerializeObject(
                new
                {
                    StatusCode = errorCode,
                    Message = message
                }));
        }
    }
}
