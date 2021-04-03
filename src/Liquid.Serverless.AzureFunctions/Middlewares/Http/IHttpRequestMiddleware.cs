using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Liquid.Serverless.AzureFunctions.Middlewares.Http
{
    /// <summary>
    /// Azure Functions Custom Http Request Middleware Pipeline Interface.
    /// </summary>
    public interface IHttpRequestMiddleware
    {
        /// <summary>
        /// Executes the action of middleware.
        /// </summary>
        /// <param name="request">The http request.</param>
        /// <param name="operation">The operation.</param>
        /// <returns></returns>
        Task InvokeAsync(HttpRequest request, Func<Task> operation);
    }
}