using System;
using System.Threading.Tasks;
using Liquid.Serverless.AzureFunctions.Extensions;
using Liquid.Core.Context;
using Liquid.Core.Utils;
using Microsoft.AspNetCore.Http;

namespace Liquid.Serverless.AzureFunctions.Middlewares.Http
{
    /// <summary>
    /// Gets the Context from request header and changes the context id.
    /// </summary>
    public class ContextHandlerMiddleware : IHttpRequestMiddleware
    {
        private const string ContextTag = "contextid";
        private readonly IHttpRequestMiddleware _next;
        private readonly ILightContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextHandlerMiddleware" /> class.
        /// </summary>
        /// <param name="next">The next http middleware.</param>
        /// <param name="context">The context.</param>
        public ContextHandlerMiddleware(IHttpRequestMiddleware next, ILightContext context)
        {
            _next = next;
            _context = context;
        }

        /// <summary>
        /// Executes the action of middleware.
        /// </summary>
        /// <param name="request">The http request.</param>
        /// <param name="operation">The operation.</param>
        /// <exception cref="ArgumentNullException">request</exception>
        public async Task InvokeAsync(HttpRequest request, Func<Task> operation)
        {
            if (request == null) { throw new ArgumentNullException(nameof(request)); }
            var contextGuid = request.GetHeaderValueFromRequest(ContextTag);
            if (!string.IsNullOrWhiteSpace(contextGuid) && contextGuid.IsGuid())
            {
                if (Guid.TryParse(contextGuid, out var contextId)) { _context.SetContextId(contextId); }
            }

            await _next.InvokeAsync(request, operation);
        }
    }
}