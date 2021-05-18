using Liquid.Serverless.AzureFunctions.Middlewares.Http;
using Liquid.Serverless.AzureFunctions.Tests.Mocks;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Liquid.Serverless.AzureFunctions.Tests.TestCases.Middlewares
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class ContextHandlerMiddlewareTests
    {
        ContextHandlerMiddleware _sut;

        /// <summary>
        /// Establishes the context.
        /// </summary>
        [SetUp]
        public void EstablishContext()
        {
            ILightContextFactoryMock.GetMock();
            _sut = new ContextHandlerMiddleware(Substitute.For<IHttpRequestMiddleware>(), ILightContextFactoryMock.Context);
        }

        [Test]
        public async Task Verify_InvokeAsync()
        {
            var mock = new DefaultHttpContext().Request;
            mock.Headers["contextid"] = "{B079DEE1-CB21-47A1-9006-982E20C562E0}";

            await _sut.InvokeAsync(mock, Substitute.For<Func<Task>>());
            Assert.AreEqual(Guid.Parse("{B079DEE1-CB21-47A1-9006-982E20C562E0}"), ILightContextFactoryMock.Context.ContextId);
        }
    }
}
