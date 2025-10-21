using OrderProcessorDemoBackendApi;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    Console.WriteLine("Received request for weather forecast");
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    BackendMetrics.RecievedRequests.Inc(1);
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
