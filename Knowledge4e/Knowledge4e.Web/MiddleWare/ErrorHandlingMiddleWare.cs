using Knowledge4e.Core.Entities;
using Knowledge4e.Core.Enums;
using Knowledge4e.Core.Properties;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Knowledge4e.Web.MiddleWare
{
    public class ErrorHandlingMiddleWare
    {
        private readonly RequestDelegate next;

        public ErrorHandlingMiddleWare(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var code = HttpStatusCode.InternalServerError;
            var result = JsonConvert.SerializeObject(
                new ServiceResult
                {
                    Data = new
                    {
                        devMsg = ex.Message,
                        cusMsg = Resources.MISA_Error
                    },
                    Messasge = Resources.MISA_Error,
                    Code = Enums.Exception
                }
                );

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(result);
        }
    }
}
