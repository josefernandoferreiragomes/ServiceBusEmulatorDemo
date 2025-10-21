using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace OrderProcessorDemoBackendApi
{
    public class BackendMetrics
    {
        public static readonly Counter RecievedRequests = Metrics.CreateCounter("demo_backend_requests_total", "Messages published by publisher");
    }
}
