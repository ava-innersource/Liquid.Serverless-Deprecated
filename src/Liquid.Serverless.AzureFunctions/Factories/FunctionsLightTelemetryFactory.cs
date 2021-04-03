using Liquid.Core.Telemetry;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Liquid.Serverless.AzureFunctions.Factories
{
    /// <summary>
    /// Gets the telemetry from the current web scope.
    /// </summary>
    /// <seealso cref="ILightTelemetryFactory" />
    [ExcludeFromCodeCoverage]
    internal class FunctionsLightTelemetryFactory : ILightTelemetryFactory
    {
        /// <summary>
        /// The context accessor
        /// </summary>
        private readonly IHttpContextAccessor _contextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionsLightTelemetryFactory"/> class.
        /// </summary>
        /// <param name="contextAccessor">The context accessor.</param>
        public FunctionsLightTelemetryFactory(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        /// <summary>
        /// Gets the telemetry.
        /// </summary>
        /// <returns></returns>
        public ILightTelemetry GetTelemetry()
        {
            return _contextAccessor.HttpContext.RequestServices.GetService<ILightTelemetry>();
        }
    }
}