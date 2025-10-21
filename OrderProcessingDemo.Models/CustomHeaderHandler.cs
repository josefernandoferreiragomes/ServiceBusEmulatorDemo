using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingDemo.Models
{
    public interface IHttpHeaderHandler
    {
        void ProcessHeaders(HttpRequestData request);
    }

    public class CustomHeaderHandler : IHttpHeaderHandler
    {
        public void ProcessHeaders(HttpRequestData request)
        {
            if (request.Headers.TryGetValues("X-Custom-Header", out var values))
            {

                var customHeader = values;
                //simulate header manipulation
                Console.WriteLine($"Custom Header Value: {customHeader}");
            }
            else
            {
                Console.WriteLine("X-Custom-Header not found.");
            }
        }
    }
}
