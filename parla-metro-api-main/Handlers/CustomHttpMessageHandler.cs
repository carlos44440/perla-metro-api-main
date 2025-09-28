using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace parla_metro_api_main.Handlers
{
    public class CustomHttpMessageHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        )
        {
            var response = await base.SendAsync(request, cancellationToken);
            response.Headers.Add("X-Correlation-ID", Guid.NewGuid().ToString());
            return response;
        }
    }
}
