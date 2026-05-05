using Microsoft.Extensions.Caching.Memory;
using N8TheGr8.Models;
using N8TheGr8.Services.Abstracts;

namespace N8TheGr8.Services;

public class InMemoryCachedWeatherService(IMemoryCache memoryCache, IWeatherService weatherService) : IWeatherService
{

    public async Task<IEnumerable<WeatherForecast>> GetWeatherForecastsAsync()
    {
        if (!memoryCache.TryGetValue("weather_forecasts", out IEnumerable<WeatherForecast>? forecasts) || forecasts is null)
        {
            forecasts = await weatherService.GetWeatherForecastsAsync() ?? Array.Empty<WeatherForecast>();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(30));

            memoryCache.Set("weather_forecasts", forecasts, cacheEntryOptions);
        }

        return forecasts;
    }
}