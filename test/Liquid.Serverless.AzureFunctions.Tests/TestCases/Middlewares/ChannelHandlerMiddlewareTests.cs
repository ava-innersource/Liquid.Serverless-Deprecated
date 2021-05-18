using Liquid.Serverless.AzureFunctions.Middlewares.Http;
using Liquid.Serverless.AzureFunctions.Tests.Mocks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Liquid.Serverless.AzureFunctions.Tests.TestCases.Middlewares
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class ChannelHandlerMiddlewareTests
    {
        ChannelHandlerMiddleware _sut;

        /// <summary>
        /// Establishes the context.
        /// </summary>
        [SetUp]
        public void EstablishContext()
        {
            ILightContextFactoryMock.GetMock();
            _sut = new ChannelHandlerMiddleware(ILightContextFactoryMock.Context);
        }

        /// <summary>
        /// Verifies the invoke asynchronous.
        /// </summary>
        [Test]
        public async Task Verify_InvokeAsync()
        {
            var mock = new DefaultHttpContext().Request;

            //Assert channel null
            await _sut.InvokeAsync(mock, Substitute.For<Func<Task>>());
            Assert.AreEqual(null, ILightContextFactoryMock.Context.ContextChannel);

            //Assert channel from queryString
            mock.Query = new QueryCollection(new Dictionary<string, StringValues> { { "channel", "web" } });
            await _sut.InvokeAsync(mock, Substitute.For<Func<Task>>());
            Assert.AreEqual("web", ILightContextFactoryMock.Context.ContextChannel);

            //Assert channel from Header
            mock.Headers["channel"] = "mobile";
            await _sut.InvokeAsync(mock, Substitute.For<Func<Task>>());
            Assert.AreEqual("mobile", ILightContextFactoryMock.Context.ContextChannel);
        }
    }
}
