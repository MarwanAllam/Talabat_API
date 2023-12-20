using System.Net;
using System.Text.Json;
using Talabat.Apis.Errors;

namespace Talabat.Apis.MiddleWares
{
    public class ExceptionMiddleWare
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionMiddleWare> logger;
        private readonly IHostEnvironment env;

        public ExceptionMiddleWare(RequestDelegate next,ILogger<ExceptionMiddleWare>logger,IHostEnvironment env)
        {
            this.next = next;
            this.logger = logger;
            this.env = env;
        }
        //Invoke Async
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (Exception ex)
            {

                logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/Json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
               
                var Response = env.IsDevelopment() ? new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace)
                    : new ApiExceptionResponse((int)HttpStatusCode.InternalServerError);
               
                var json=JsonSerializer.Serialize(Response);
                await context.Response.WriteAsync(json); 
                
            }
        }


    }
}
