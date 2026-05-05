using N8TheGr8.Models;
using N8TheGr8.Services.Abstracts;

namespace N8TheGr8.Services.Impl;

public class TestWeatherService : IWeatherService
{
    public async Task<IEnumerable<WeatherForecast>> GetWeatherForecastsAsync()
    {
        var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = "Test Summary"
        })
        .ToArray();

        return await Task.FromResult(forecasts.AsEnumerable());
    }
}