using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Gym_Core.Exceptions;

namespace Gym_Core.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionMiddleware> logger;
        private readonly IHostEnvironment env;

        public ExceptionMiddleware(RequestDelegate next,
            ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            this.next = next;
            this.logger = logger;
            this.env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (AppException e)
            {
                logger.LogWarning(
                    "AppException ({StatusCode}) on {Method} {Path} [TraceId: {TraceId}]: {Message}",
                    e.StatusCode,
                    context.Request.Method,
                    context.Request.Path,
                    context.TraceIdentifier,
                    e.Message
                );
                context.Response.StatusCode = e.StatusCode;
                await context.Response.WriteAsJsonAsync(new
                {
                    StatusCode = e.StatusCode,
                    Message = e.Message
                });
            }
            catch (Exception e)
            {
                logger.LogError(
                    e,
                    "Unhandled exception on {Method} {Path} [TraceId: {TraceId}]",
                    context.Request.Method,
                    context.Request.Path,
                    context.TraceIdentifier
                );
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new
                {
                    StatusCode = 500,
                    Message = "Something went wrong.",
                    Details = env.IsDevelopment() ? e.Message : null
                });
            }
        }
    }
}
