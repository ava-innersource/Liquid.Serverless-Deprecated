using System;
using System.Diagnostics.CodeAnalysis;
using Liquid.Core.Context;
using Liquid.Core.Localization;
using Liquid.Core.Telemetry;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Liquid.Serverless.AzureFunctions
{
    /// <summary>
    /// Base Function Class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class BaseFunction
    {
        /// <summary>
        /// Gets or sets the service provider.
        /// </summary>
        /// <value>
        /// The service provider.
        /// </value>
        protected IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected ILightContext Context { get; }

        /// <summary>
        /// Gets or sets the log service.
        /// </summary>
        /// <value>
        /// The log service.
        /// </value>
        protected ILogger LogService { get; }

        /// <summary>
        /// Gets or sets the mediator service.
        /// </summary>
        /// <value>
        /// The mediator service.
        /// </value>
        protected IMediator Mediator { get; }

        /// <summary>
        /// Gets the telemetry.
        /// </summary>
        /// <value>
        /// The telemetry.
        /// </value>
        protected ILightTelemetry Telemetry { get; }

        /// <summary>
        /// Gets the localization service.
        /// </summary>
        /// <value>
        /// The localization service.
        /// </value>
        protected ILocalization Localization { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFunction"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="mediator">The mediator.</param>
        /// <param name="context">The context.</param>
        /// <param name="telemetry">The telemetry.</param>
        /// <param name="localizationService">The localization service.</param>
        protected BaseFunction(IServiceProvider serviceProvider,
                               ILoggerFactory loggerFactory,
                               IMediator mediator,
                               ILightContext context,
                               ILightTelemetry telemetry,
                               ILocalization localizationService)
        {
            ServiceProvider = serviceProvider;
            LogService = loggerFactory.CreateLogger(GetType().Name);
            Mediator = mediator;
            Context = context;
            Telemetry = telemetry;
            Localization = localizationService;
        }
    }
}