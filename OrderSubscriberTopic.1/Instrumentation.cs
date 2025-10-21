using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Prometheus;

public static class SubscriberMetrics
{
    // Counters
    public static readonly Counter MessagesReceived = Metrics.CreateCounter(
        "subscriber_messages_received_total",
        "Total number of messages successfully processed by subscriber",
        new CounterConfiguration { LabelNames = new[] { "topic", "subscription" } });

    public static readonly Counter MessagesFailed = Metrics.CreateCounter(
        "subscriber_messages_failed_total",
        "Total number of messages that failed processing",
        new CounterConfiguration { LabelNames = new[] { "topic", "subscription" } });

    public static readonly Counter DeadletterMessages = Metrics.CreateCounter(
        "servicebus_deadletter_messages_total",
        "Messages moved to dead letter",
        new CounterConfiguration { LabelNames = new[] { "topic", "subscription" } });

    // Histogram for processing duration - add labels so Grafana queries with topic/subscription work
    public static readonly Histogram ProcessingDuration = Metrics.CreateHistogram(
        "subscriber_processing_duration_seconds",
        "Histogram of message processing durations in seconds",
        new HistogramConfiguration
        {
            Buckets = Histogram.LinearBuckets(start: 0.01, width: 0.05, count: 20),
            LabelNames = new[] { "topic", "subscription" }
        });

    // Dependency instrumentation (mock API)
    public static readonly Counter HttpDependencyCalls = Metrics.CreateCounter(
        "http_dependency_calls_total",
        "HTTP dependency calls",
        new CounterConfiguration { LabelNames = new[] { "dependency", "status" } });

    // Add dependency label to histogram so queries like {dependency="order-processor-backend"} work
    public static readonly Histogram HttpDependencyDuration = Metrics.CreateHistogram(
        "http_dependency_duration_seconds",
        "HTTP dependency call duration seconds",
        new HistogramConfiguration
        {
            Buckets = Histogram.ExponentialBuckets(0.01, 2, 10),
            LabelNames = new[] { "dependency" }
        });

    // Helper to wrap processing
    public static async Task ProcessWithMetricsAsync(string topic, string subscription, Func<Task> work)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            await work();
            sw.Stop();
            // Observe with labels
            ProcessingDuration.WithLabels(topic, subscription).Observe(sw.Elapsed.TotalSeconds);
            MessagesReceived.WithLabels(topic, subscription).Inc();
        }
        catch (Exception)
        {
            sw.Stop();
            ProcessingDuration.WithLabels(topic, subscription).Observe(sw.Elapsed.TotalSeconds);
            MessagesFailed.WithLabels(topic, subscription).Inc();
            throw;
        }
    }

    // Example dependency call helper
    public static async Task<T> CallDependencyAsync<T>(string dependencyName, Func<Task<T>> call)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var result = await call();
            sw.Stop();

            // Record duration with dependency label
            HttpDependencyDuration.WithLabels(dependencyName).Observe(sw.Elapsed.TotalSeconds);

            // If the call returned an HttpResponseMessage, capture status code, otherwise assume 200
            if (result is HttpResponseMessage httpResp)
            {
                var statusCode = ((int)httpResp.StatusCode).ToString();
                HttpDependencyCalls.WithLabels(dependencyName, statusCode).Inc();
            }
            else
            {
                HttpDependencyCalls.WithLabels(dependencyName, "200").Inc();
            }

            return result;
        }
        catch (Exception)
        {
            sw.Stop();
            HttpDependencyDuration.WithLabels(dependencyName).Observe(sw.Elapsed.TotalSeconds);
            HttpDependencyCalls.WithLabels(dependencyName, "500").Inc();
            throw;
        }
    }
}