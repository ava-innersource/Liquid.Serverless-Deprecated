using System.IO;
using Liquid.Serverless.AzureFunctions.Configuration;
using Liquid.Serverless.AzureFunctions.Factories;
using Liquid.Serverless.AzureFunctions.Middlewares.Http;
using Liquid.Core.Configuration;
using Liquid.Core.Context;
using Liquid.Core.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using Liquid.Core.Telemetry;

namespace Liquid.Serverless.AzureFunctions.Extensions
{
    /// <summary>
    /// Functions Startup Extensions Class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class StartupExtensions
    {
        /// <summary>
        /// Configures the liquid functions.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IFunctionsHostBuilder ConfigureLiquidFunctions(this IFunctionsHostBuilder builder)
        {
            //This step is necessary to obtain the appsettings in a different path than default application running path.
            //In Azure Functions, the assembly runs in a different folder.
            var appDirectory = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<ExecutionContextOptions>>().Value.AppDirectory;
            IConfiguration configurationRoot = new ConfigurationBuilder().AddJsonFile(Path.Combine(appDirectory, "appsettings.json")).Build();

            builder.Services.AddSingleton(configurationRoot);
            builder.Services.AddSingleton<ILightConfiguration<FunctionSettings>, FunctionConfiguration>();

            builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddHttpClient();
            builder.Services.AddLocalizationService();
            builder.Services.AddScoped<ILightContext, LightContext>();
            builder.Services.AddTransient<ILightContextFactory, FunctionsLightContextFactory>();

            builder.Services.AddScoped<ILightTelemetry, LightTelemetry>();
            builder.Services.AddTransient<ILightTelemetryFactory, FunctionsLightTelemetryFactory>();

            //Adds the Middleware chain of execution.
            builder.Services.Chain<IHttpRequestMiddleware>()
                            .Add<ContextHandlerMiddleware>()
                            .Add<TelemetryHandlerMiddleware>()
                            .Add<ExceptionHandlerMiddleware>()
                            .Add<CultureHandlerMiddleware>()
                            .Add<ChannelHandlerMiddleware>()
                            .Configure();

            return builder;
        }
    }
}