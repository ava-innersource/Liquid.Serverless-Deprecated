using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Liquid.Serverless.AzureFunctions.Extensions;
using Liquid.Core.Configuration;
using Liquid.Core.Context;
using Liquid.Core.Localization;
using Microsoft.AspNetCore.Http;

namespace Liquid.Serverless.AzureFunctions.Middlewares.Http
{
    /// <summary>
    /// Handles the culture code information from request. Checks the culture code either from header or querystring.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class CultureHandlerMiddleware : IHttpRequestMiddleware
    {
        private const string CultureTag = "culture";
        private readonly ILightContext _context;
        private readonly ILightConfiguration<CultureSettings> _cultureSettings;
        private readonly IHttpRequestMiddleware _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="CultureHandlerMiddleware" /> class.
        /// </summary>
        /// <param name="next">The next.</param>
        /// <param name="context">The context.</param>
        /// <param name="cultureSettings">The culture settings.</param>
        public CultureHandlerMiddleware(IHttpRequestMiddleware next, ILightContext context, ILightConfiguration<CultureSettings> cultureSettings)
        {
            _next = next;
            _context = context;
            _cultureSettings = cultureSettings;
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
            var cultureCode = request.GetHeaderValueFromRequest(CultureTag);
            if (string.IsNullOrEmpty(cultureCode)) { cultureCode = request.GetValueFromQuerystring(CultureTag); }
            if (string.IsNullOrEmpty(cultureCode) && !string.IsNullOrEmpty(_cultureSettings.Settings.DefaultCulture)) 
            {
                cultureCode = _cultureSettings.Settings.DefaultCulture;
            }

            if (!string.IsNullOrEmpty(cultureCode))
            {
                try
                {
                    _context.SetCulture(cultureCode);
                }
                catch
                {
                    // ignored. Left intentionally blank.
                }
            }
            await _next.InvokeAsync(request, operation);
        }
    }
}