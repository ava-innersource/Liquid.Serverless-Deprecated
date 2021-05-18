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
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class CultureHandlerMiddlewareTests
    {
        CultureHandlerMiddleware _sut;

        /// <summary>
        /// Establishes the context.
        /// </summary>
        [SetUp]
        public void EstablishContext()
        {
            ILightContextFactoryMock.GetMock();
            _sut = new CultureHandlerMiddleware(Substitute.For<IHttpRequestMiddleware>(),
                                                ILightContextFactoryMock.Context,
                                                ILightConfigurationCultureSettingsMock.GetMock());
        }

        [Test]
        public async Task Verify_InvokeAsync()
        {
            var mock = new DefaultHttpContext().Request;

            //Assert default culture.
            await _sut.InvokeAsync(mock, Substitute.For<Func<Task>>());
            Assert.AreEqual("en-US", ILightContextFactoryMock.Context.ContextCulture);

            //Assert culture from queryString
            mock.Query = new QueryCollection(new Dictionary<string, StringValues> { { "culture", "pt-BR" } });
            await _sut.InvokeAsync(mock, Substitute.For<Func<Task>>());
            Assert.AreEqual("pt-BR", ILightContextFactoryMock.Context.ContextCulture);

            //Assert culture from Header
            mock.Headers["culture"] = "fr-FR";
            await _sut.InvokeAsync(mock, Substitute.For<Func<Task>>());
            Assert.AreEqual("fr-FR", ILightContextFactoryMock.Context.ContextCulture);
        }
    }
}
