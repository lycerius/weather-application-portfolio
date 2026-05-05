using Microsoft.AspNetCore.Mvc;
using N8TheGr8.Models;
using N8TheGr8.Services.Abstracts;

namespace N8TheGr8.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController([FromKeyedServices("InMemoryCachedWeatherService")] IWeatherService weatherService) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        return await weatherService.GetWeatherForecastsAsync();
    }
}
