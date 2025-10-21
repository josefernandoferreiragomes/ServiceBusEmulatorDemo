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

    // Histogram for processing duration (topic/subscription labels)
    public static readonly Histogram ProcessingDuration = Metrics.CreateHistogram(
        "subscriber_processing_duration_seconds",
        "Histogram of message processing durations in seconds",
        new HistogramConfiguration
        {
            Buckets = Histogram.LinearBuckets(start: 0.01, width: 0.05, count: 20),
            LabelNames = new[] { "topic", "subscription" }
        });

    // Dependency instrumentation with caller label
    // Labels: caller = who invoked, dependency = target service, status = HTTP status
    public static readonly Counter HttpDependencyCalls = Metrics.CreateCounter(
        "http_dependency_calls_total",
        "HTTP dependency calls",
        new CounterConfiguration { LabelNames = new[] { "caller", "dependency", "status" } });

    public static readonly Histogram HttpDependencyDuration = Metrics.CreateHistogram(
        "http_dependency_duration_seconds",
        "HTTP dependency call duration seconds",
        new HistogramConfiguration
        {
            Buckets = Histogram.ExponentialBuckets(0.01, 2, 10),
            LabelNames = new[] { "caller", "dependency" }
        });

    // Helper to wrap processing
    public static async Task ProcessWithMetricsAsync(string topic, string subscription, Func<Task> work)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            await work();
            sw.Stop();
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

    // Example dependency call helper including callerName
    public static async Task<T> CallDependencyAsync<T>(string callerName, string dependencyName, Func<Task<T>> call)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var result = await call();
            sw.Stop();

            // Observe duration with caller + dependency
            HttpDependencyDuration.WithLabels(callerName, dependencyName).Observe(sw.Elapsed.TotalSeconds);

            if (result is HttpResponseMessage httpResp)
            {
                var statusCode = ((int)httpResp.StatusCode).ToString();
                HttpDependencyCalls.WithLabels(callerName, dependencyName, statusCode).Inc();
            }
            else
            {
                HttpDependencyCalls.WithLabels(callerName, dependencyName, "200").Inc();
            }

            return result;
        }
        catch (Exception)
        {
            sw.Stop();
            HttpDependencyDuration.WithLabels(callerName, dependencyName).Observe(sw.Elapsed.TotalSeconds);
            HttpDependencyCalls.WithLabels(callerName, dependencyName, "500").Inc();
            throw;
        }
    }
}