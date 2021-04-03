using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Liquid.Serverless.AzureFunctions.Configuration;
using Liquid.Serverless.AzureFunctions.Extensions;
using Liquid.Core.Configuration;
using Liquid.Core.Telemetry;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Liquid.Serverless.AzureFunctions.Middlewares.Http
{
    /// <summary>
    /// Executes the telemetry of the request and track all data executed on request.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class TelemetryHandlerMiddleware : IHttpRequestMiddleware
    {
        private readonly IHttpRequestMiddleware _next;
        private readonly ILightTelemetry _telemetry;
        private readonly ILightConfiguration<FunctionSettings> _apiSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandlerMiddleware" /> class.
        /// </summary>
        /// <param name="next">The next.</param>
        /// <param name="telemetry">The telemetry.</param>
        /// <param name="apiSettings">The API settings.</param>
        public TelemetryHandlerMiddleware(IHttpRequestMiddleware next, ILightTelemetry telemetry, ILightConfiguration<FunctionSettings> apiSettings)
        {
            _next = next;
            _telemetry = telemetry;
            _apiSettings = apiSettings;
        }

        /// <summary>
        /// Executes the action of middleware.
        /// </summary>
        /// <param name="request">The http request.</param>
        /// <param name="operation">The operation.</param>
        /// <exception cref="ArgumentNullException">request</exception>
        public async Task InvokeAsync(HttpRequest request, Func<Task> operation)
        {
            _telemetry.AddContext("HttpRequest");
            if (_apiSettings?.Settings?.TrackRequests == true)
            {
                var requestBodyText = string.Empty;
                var originalBody = request.HttpContext.Response.Body;
                var memStream = new MemoryStream();
                string responseBodyText = string.Empty;
                try
                {
                    
                        request.HttpContext.Response.Body = memStream;

                        _telemetry.StartTelemetryStopWatchMetric("RequestTracking");
                        requestBodyText = await request.GetRequestBody();
                        await _next.InvokeAsync(request, operation);

                        memStream.Position = 0;
                        responseBodyText = new StreamReader(memStream).ReadToEnd();

                        memStream.Position = 0;
                        await memStream.CopyToAsync(originalBody);
                    
                }
                finally
                {
                    try
                    {
                        var response = request.HttpContext.Response;
                        response.Body = originalBody;
                        
                        var trackingObject = new
                        {
                            httpRequest = new
                            {
                                method = request.Method,
                                url = request.GetDisplayUrl(),
                                headers = request.Headers,
                                body = requestBodyText,
                                size = requestBodyText?.Length
                            },
                            httpResponse = new
                            {
                                statuscode = response?.StatusCode,
                                headers = response?.Headers,
                                body = responseBodyText,
                                size = responseBodyText?.Length
                            }
                        };

                        _telemetry.CollectTelemetryStopWatchMetric("RequestTracking", trackingObject);
                        _telemetry.RemoveContext("HttpRequest");
                        _telemetry.Flush();
                    }
                    catch
                    {
                        // ignored. Left intentionally blank.
                    }
                }
            }
            else
            {
                try
                {
                    await _next.InvokeAsync(request, operation);
                }
                finally
                {
                    _telemetry.RemoveContext("HttpRequest");
                    _telemetry.Flush();
                }
            }
        }
    }
}