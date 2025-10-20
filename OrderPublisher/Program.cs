using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Prometheus;
using Microsoft.AspNetCore.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

// Configure functions web application (existing behavior)
builder.ConfigureFunctionsWebApplication();

// Keep Application Insights config
builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

// Build and start a tiny Kestrel-based metrics host inside the same process.
// Prometheus.AspNetCore provides UseHttpMetrics() and MapMetrics().
var metricsBuilder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder();
metricsBuilder.WebHost.ConfigureKestrel(options =>
{
    // Listen on port 9184 for metrics (internal container port)
    options.ListenAnyIP(9184);
});

// We only need routing and the metrics middleware
var metricsApp = metricsBuilder.Build();
metricsApp.UseRouting();
metricsApp.UseHttpMetrics();   // optional: captures basic http metrics
metricsApp.MapMetrics();      // exposes /metrics

// Start metrics host in background
_ = metricsApp.StartAsync();

// Build and run the Functions host as before (blocking)
builder.Build().Run();