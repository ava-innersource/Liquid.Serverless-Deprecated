using Liquid.Core.Context;
using Liquid.Core.Localization;
using Liquid.Core.Telemetry;
using Liquid.Serverless.AzureFunctions;
using Liquid.Serverless.AzureFunctions.Middlewares.Http;
using Liquid.Serverless.AzureFunctions.Tests.Mocks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Liquid.Serverless.AzureFunctions.Tests.TestCases.Functions
{
    /// <summary>
    /// Base controller test case
    /// </summary>
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class BaseHttpTriggerFunctionTests
    {
        

        /// <summary>
        /// Verifies the execute asynchronous.
        /// </summary>
        [Test]
        public async Task Verify_ExecuteAsync()
        {
            var sut = new FunctionTest(new ServiceCollection().BuildServiceProvider(),
                                       ILoggerFactoryMock.GetMock(),
                                       IMediatorMock.GetMock(),
                                       ILightContextMock.GetMock(),
                                       ILightTelemetryMock.GetMock(),
                                       ILocalizationMock.GetMock(),
                                       new ChannelHandlerMiddleware(new LightContext()));
            await sut.TestExecuteAsync();
        }

        /// <summary>
        /// Verifies the execute asynchronous with Validation Errors.
        /// </summary>
        [Test]
        public async Task Verify_ExecuteAsync_WithValidatiorErrors()
        {
            var sut = new FunctionTest(new ServiceCollection().BuildServiceProvider(),
                                       ILoggerFactoryMock.GetMock(),
                                       IMediatorMock.GetMock(),
                                       ILightContextMock.GetMockWithValidationErrors(),
                                       ILightTelemetryMock.GetMock(),
                                       ILocalizationMock.GetMock(),
                                       new ChannelHandlerMiddleware(new LightContext()));

            var result = await sut.TestExecuteExceptionAsync();
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        /// <summary>
        /// Verifies the execute asynchronous with custom Error.
        /// </summary>
        [Test]
        public async Task Verify_ExecuteAsync_WithCustomErrors()
        {
            var sut = new FunctionTest(new ServiceCollection().BuildServiceProvider(),
                                       ILoggerFactoryMock.GetMock(),
                                       IMediatorMock.GetMock(),
                                       ILightContextMock.GetMockWithCustomErrors(),
                                       ILightTelemetryMock.GetMock(),
                                       ILocalizationMock.GetMock(),
                                       new ChannelHandlerMiddleware(new LightContext()));

            var result = await sut.TestExecuteExceptionAsync();
            Assert.IsInstanceOf(typeof(ObjectResult), result);
            Assert.AreEqual(404, ((ObjectResult)result).StatusCode);
        }
    }

    /// <summary>
    /// Dummy Controller for test purpose.
    /// </summary>
    /// <seealso cref="Http.Controllers.BaseController" />
    internal class FunctionTest : BaseHttpTriggerFunction
    {
        public FunctionTest(IServiceProvider serviceProvider,
                            ILoggerFactory loggerFactory,
                            IMediator mediator,
                            ILightContext context,
                            ILightTelemetry telemetry,
                            ILocalization localizationService,
                            IHttpRequestMiddleware requestMiddleware)
            : base(serviceProvider, loggerFactory, mediator, context, telemetry, localizationService, requestMiddleware)
        {
        }


        /// <summary>
        /// Tests the execute asynchronous.
        /// </summary>
        public async Task TestExecuteAsync()
        {
            await ExecuteAsync(new DefaultHttpContext().Request, Substitute.For<IRequest<bool>>());
            await ExecuteAsync(new DefaultHttpContext().Request, Substitute.For<IRequest<bool>>(), System.Net.HttpStatusCode.OK);
        }

        public async Task<IActionResult> TestExecuteExceptionAsync()
        {
            return await ExecuteAsync(new DefaultHttpContext().Request, Substitute.For<IRequest<bool>>());
        }
    }
}
