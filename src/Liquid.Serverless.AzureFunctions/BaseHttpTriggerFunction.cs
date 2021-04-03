using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using Liquid.Serverless.AzureFunctions.Middlewares.Http;
using Liquid.Core.Context;
using Liquid.Core.Exceptions;
using Liquid.Core.Localization;
using Liquid.Core.Telemetry;
using Liquid.Core.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Liquid.Serverless.AzureFunctions
{
    /// <summary>
    /// Base Http Trigger Function Class.
    /// </summary>
    /// <seealso cref="Liquid.Serverless.AzureFunctions.BaseFunction" />
    public abstract class BaseHttpTriggerFunction : BaseFunction
    {
        private readonly IHttpRequestMiddleware _requestMiddleware;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseHttpTriggerFunction" /> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="mediator">The mediator.</param>
        /// <param name="context">The context.</param>
        /// <param name="telemetry">The telemetry.</param>
        /// <param name="localizationService">The resource catalog.</param>
        /// <param name="requestMiddleware">The request middleware.</param>
        protected BaseHttpTriggerFunction(IServiceProvider serviceProvider,
                                          ILoggerFactory loggerFactory,
                                          IMediator mediator,
                                          ILightContext context,
                                          ILightTelemetry telemetry,
                                          ILocalization localizationService,
                                          IHttpRequestMiddleware requestMiddleware) : base(serviceProvider, loggerFactory, mediator, context, telemetry, localizationService)
        {
            _requestMiddleware = requestMiddleware;
        }

        /// <summary>
        /// Executes the action and returns the response using http response code 200 (Success).
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <param name="request">The request command or query.</param>
        /// <returns></returns>
        protected virtual async Task<IActionResult> ExecuteAsync<TRequest>(HttpRequest httpRequest, IRequest<TRequest> request)
        {
            return await ExecuteAsync(httpRequest, request, HttpStatusCode.OK);
        }

        /// <summary>
        /// Executes the action and returns the response using a custom http response code.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <param name="request">The request command or query.</param>
        /// <param name="responseCode">The http response code.</param>
        /// <returns></returns>
        protected virtual async Task<IActionResult> ExecuteAsync<TRequest>(HttpRequest httpRequest, IRequest<TRequest> request, HttpStatusCode responseCode)
        {
            IActionResult response = null;

            using (ServiceProvider.CreateScope())
            {
                await _requestMiddleware.InvokeAsync(httpRequest, async () => response = await GenerateResponseAsync(async () => await Mediator.Send(request), responseCode));
            }

            return response;
        }

        /// <summary>
        /// Generates the response asynchronous.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="requestFunction">The request function.</param>
        /// <param name="responseCode">The response code.</param>
        /// <returns></returns>
        private async Task<IActionResult> GenerateResponseAsync<TResponse>(Func<Task<TResponse>> requestFunction, HttpStatusCode responseCode)
        {
            try
            {
                Telemetry.AddContext("ExecuteAsync");
                var response = await requestFunction();
                var messages = Context.GetNotifications();

                return messages.Any() ?
                    new ObjectResult(new { response, messages }) { StatusCode = (int?)responseCode } :
                    new ObjectResult(new { response }) { StatusCode = (int?)responseCode };
            }
            catch (ValidationException validationException)
            {
                return HandleValidationException(validationException);
            }
            catch (LightCustomException ex)
            {
                Telemetry.AddErrorTelemetry(ex);
                return new ObjectResult(new { messages = Localization.Get(ex.Message) }) { StatusCode = ex.ResponseCode.Value };
            }
            finally
            {
                Telemetry.RemoveContext("ExecuteAsync");
            }
        }

        /// <summary>
        /// Handles the validation exception result.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        private IActionResult HandleValidationException(ValidationException ex)
        {
            var messages = new Dictionary<string, string>();
            var index = 0;

            ex.Errors.Each(error =>
            {
                messages.Add($"{index}_{error.PropertyName}", Localization.Get(error.ErrorMessage, Context.ContextChannel));
                index++;
            });
            return new BadRequestObjectResult(new { messages });
        }
    }
}
