using N8TheGr8.Models;

namespace N8TheGr8.Services.Abstracts;

public interface IWeatherService
{
    Task<IEnumerable<WeatherForecast>> GetWeatherForecastsAsync();
}
