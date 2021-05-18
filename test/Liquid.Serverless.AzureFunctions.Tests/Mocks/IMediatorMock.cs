using MediatR;
using NSubstitute;
using System.Diagnostics.CodeAnalysis;

namespace Liquid.Serverless.AzureFunctions.Tests.Mocks
{
    /// <summary>
    /// IMediator Mock, returns the mock interface for tests purpose.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class IMediatorMock
    {
        /// <summary>
        /// Gets the IMediator mock.
        /// </summary>
        /// <returns></returns>
        public static IMediator GetMock()
        {
            var mock = Substitute.For<IMediator>();
            return mock;
        }
    }
}
