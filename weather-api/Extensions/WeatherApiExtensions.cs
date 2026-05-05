using Microsoft.Extensions.Caching.Memory;
using N8TheGr8.Services;
using N8TheGr8.Services.Abstracts;
using N8TheGr8.Services.Impl;

namespace N8TheGr8.Extensions;

public static class WeatherApiExtensions
{
    public static void AddWeatherServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddMemoryCache();
        builder.Services.AddKeyedScoped<IWeatherService, TestWeatherService>("TestWeatherService");
        builder.Services.AddKeyedScoped<IWeatherService>("InMemoryCachedWeatherService", (sp, key) =>
        {
            var memoryCache = sp.GetRequiredService<IMemoryCache>();
            var weatherService = sp.GetRequiredKeyedService<IWeatherService>("TestWeatherService");
            return new InMemoryCachedWeatherService(memoryCache, weatherService);
        });


    }
}