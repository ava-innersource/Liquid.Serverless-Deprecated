using FluentValidation;
using FluentValidation.Results;
using Liquid.Core.Context;
using Liquid.Core.Exceptions;
using NSubstitute;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Liquid.Serverless.AzureFunctions.Tests.Mocks
{
    /// <summary>
    /// ILightContext Mock, returns the mock interface for tests purpose.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ILightContextMock
    {
        /// <summary>
        /// Gets the ILightContext mock.
        /// </summary>
        /// <returns></returns>
        public static ILightContext GetMock()
        {
            var mock = Substitute.For<ILightContext>();
            return mock;
        }

        public static ILightContext GetMockWithValidationErrors()
        {
            var mock = Substitute.For<ILightContext>();
            mock.When(x => x.GetNotifications())
                .Do(x => throw new ValidationException(new List<ValidationFailure>() { new ValidationFailure("test", "errorMessage") }));
            return mock;
        }

        public static ILightContext GetMockWithCustomErrors()
        {
            var mock = Substitute.For<ILightContext>();
            mock.When(x => x.GetNotifications())
                .Do(x => throw new LightCustomException("custom exception", ExceptionCustomCodes.NotFound));
            return mock;
        }
    }
}
