# DEPRECATED [![No Maintenance Intended](http://unmaintained.tech/badge.svg)](http://unmaintained.tech/)
**This repo is no longer supported and will not receive any updates nor bug fixes. We've made significative breaking changes on the stable version of Liquid Application Framework core components, and this component is no longer supported and is not expected to be refactored to compose the 2.0 release.**

Liquid Application Framework - Serverless
=========================================

This repository is part of the [Liquid Application Framework](https://github.com/Avanade/Liquid-Application-Framework), a modern Dotnet Core Application Framework for building cloud native microservices.

The main repository contains the examples and documentation on how to use Liquid.

Liquid Serverless
-----------------

This package contains the serverless subsystem of Liquid, for building serverless projects

|Available implementations|Badges| |
|:--|--|--|
|[Liquid.Serverless.AzureFunctions](https://github.com/Avanade/Liquid.Serverless/tree/main/src/Liquid.Serverless.AzureFunctions)|[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Avanade_Liquid.Serverless.AzureFunctions&metric=alert_status)](https://sonarcloud.io/dashboard?id=Avanade_Liquid.Serverless.AzureFunctions)| Implementation to use liquid to build Azure Functions.|

Usage
-----

To create an Azure Function, setup your dependencies in the Startup of the functions

```csharp
using Liquid.Serverless.AzureFunctions.Extensions;
using Liquid.Domain.Extensions;
using Liquid.Sample.Domain.Commands;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Liquid.Core.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Liquid.Sample.AzureFunctions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.ConfigureLiquidFunctions();
            builder.Services.AddAutoMapper(typeof(HelloCommand).Assembly);
            builder.Services.AddDomainRequestHandlers(typeof(HelloCommand).Assembly);
            // Add additional dependencies for injection as needed
        }
    }
} 
```

For each Function, you need to create the corresponding class:

```csharp
using System.Net;
using System.Threading.Tasks;
using Liquid.Serverless.AzureFunctions;
using Liquid.Core.Utils;
using Liquid.Sample.Domain.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System;
using Microsoft.Extensions.Logging;
using MediatR;
using Liquid.Core.Context;
using Liquid.Core.Telemetry;
using Liquid.Core.Localization;
using Liquid.Serverless.AzureFunctions.Middlewares.Http;

namespace Liquid.Sample.AzureFunctions.Functions
{

    public class HelloFunction : BaseHttpTriggerFunction
    {
        public HelloFunction(IServiceProvider serviceProvider,
                                  ILoggerFactory loggerFactory,
                                  IMediator mediator,
                                  ILightContext context,
                                  ILightTelemetry telemetry,
                                  ILocalization localizationService,
                                  IHttpRequestMiddleware requestMiddleware) 
            : base(serviceProvider, loggerFactory, mediator, context, telemetry, localizationService, requestMiddleware)
        {
        }

        [FunctionName("HelloWorld")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            var requestBody = req.Body.AsStringUtf8();
            var request = requestBody.ParseJson<HelloCommand>();
            return await ExecuteAsync(req, request, HttpStatusCode.Created);
        }
    }
}
```

To view how to build the command handlers, access the [Liquid Domain](https://github.com/Avanade/Liquid.Domain) project.
