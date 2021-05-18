using System.Diagnostics.CodeAnalysis;
using Liquid.Core.Configuration;
using Microsoft.Extensions.Configuration;

namespace Liquid.Serverless.AzureFunctions.Configuration
{
    /// <summary>
    /// Functions Configuration Class.
    /// </summary>
    /// <seealso>
    ///     <cref>Liquid.Core.Configuration.LightConfiguration</cref>
    /// </seealso>
    /// <seealso>
    ///     <cref>Liquid.Core.Configuration.ILightConfiguration{Liquid.Serverless.AzureFunctions.Configuration.FunctionSettings}</cref>
    /// </seealso>
    [ExcludeFromCodeCoverage]
    [LiquidConfigurationSection("azureFunctions")]
    public class FunctionConfiguration : LightConfiguration, ILightConfiguration<FunctionSettings>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionConfiguration"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public FunctionConfiguration(IConfiguration configuration) : base(configuration)
        {
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        public FunctionSettings Settings => GetConfigurationSection<FunctionSettings>();
    }
}