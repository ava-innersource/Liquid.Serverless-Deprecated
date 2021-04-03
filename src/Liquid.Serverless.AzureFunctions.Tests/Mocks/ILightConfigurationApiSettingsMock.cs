using Liquid.Core.Configuration;
using Liquid.Serverless.AzureFunctions.Configuration;
using NSubstitute;
using System.Diagnostics.CodeAnalysis;

namespace Liquid.Serverless.AzureFunctions.Tests.Mocks
{
    /// <summary>
    /// ILightConfigurationApiSettings Mock, returns the mock interface for tests purpose.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ILightConfigurationApiSettingsMock
    {
        /// <summary>
        /// Gets the ILightConfigurationApiSettings mock.
        /// </summary>
        /// <returns></returns>
        public static ILightConfiguration<FunctionSettings> GetMock()
        {
            var mock = Substitute.For<ILightConfiguration<FunctionSettings>>();
            mock.Settings.Returns(new FunctionSettings { ShowDetailedException = true, TrackRequests = true });
            return mock;
        }
    }
}
