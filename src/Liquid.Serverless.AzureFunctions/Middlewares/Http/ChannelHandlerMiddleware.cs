using System;
using System.Threading.Tasks;
using Liquid.Serverless.AzureFunctions.Extensions;
using Liquid.Core.Context;
using Microsoft.AspNetCore.Http;

namespace Liquid.Serverless.AzureFunctions.Middlewares.Http
{
    /// <summary>
    /// Channel Handler Middleware class.
    /// </summary>
    public class ChannelHandlerMiddleware : IHttpRequestMiddleware
    {
        private const string ChannelTag = "channel";
        private readonly ILightContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelHandlerMiddleware" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ChannelHandlerMiddleware(ILightContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Executes the action of middleware.
        /// </summary>
        /// <param name="request">The http request.</param>
        /// <param name="operation">The operation.</param>
        /// <exception cref="ArgumentNullException">context</exception>
        public async Task InvokeAsync(HttpRequest request, Func<Task> operation)
        {
            if (request == null) { throw new ArgumentNullException(nameof(request)); }
            var channelCode = request.GetHeaderValueFromRequest(ChannelTag);
            if (string.IsNullOrEmpty(channelCode)) { channelCode = request.GetValueFromQuerystring(ChannelTag); }
            if (string.IsNullOrEmpty(channelCode)) { channelCode = string.Empty; }
            SetCurrentChannel(channelCode);

            //Channel is the last of chain. Executes the operation
            await operation();
        }

        /// <summary>
        /// Sets the current channel.
        /// </summary>
        /// <param name="channelCode">The channel code.</param>
        private void SetCurrentChannel(string channelCode)
        {
            if (string.IsNullOrEmpty(channelCode)) return;

            try
            {
                _context.SetChannel(channelCode);
            }
            catch
            {
                // ignored. Left intentionally blank.
            }
        }
    }
}
