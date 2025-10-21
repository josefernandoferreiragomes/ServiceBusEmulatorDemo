using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace OrderProcessingDemo.Models
{
    public class CustomHeaderMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly IHttpHeaderHandler _headerHandler;

        public CustomHeaderMiddleware(IHttpHeaderHandler headerHandler)
        {
            _headerHandler = headerHandler;
        }

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var request = await context.GetHttpRequestDataAsync();
            if (request != null)
            {
                _headerHandler.ProcessHeaders(request);
            }

            await next(context);
        }
    }
}
