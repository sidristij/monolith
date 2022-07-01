using System;
using System.Linq;
using System.Threading.Tasks;

namespace First.Services
{
    public class FirstWeatherService : IFirstWeatherService
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public Task<FirstWeatherForecast[]> GetForecastAsync()
        {
            var rng = new Random();
            var result = Enumerable
                .Range(1, 5)
                .Select(_ => rng.Next(-20, 55))
                .Select((index, x) => new FirstWeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = x,
                    TemperatureF = 32 + (int)(x / 0.5556),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
            
            return Task.FromResult(result);
        }
    }
}
