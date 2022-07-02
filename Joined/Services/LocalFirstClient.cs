using System.Linq;
using System.Threading.Tasks;
using First.Services;
using Second.Services;

namespace Joined.Services
{
    public class LocalFirstClient : IFirstClient
    {
        private readonly IFirstWeatherService _firstWeatherService;

        public LocalFirstClient(IFirstWeatherService firstWeatherService)
        {
            _firstWeatherService = firstWeatherService;
        }

        public async Task<FirstInSecondWeatherForecast[]> GetForecastAsync()
        {
            var data = await _firstWeatherService.GetForecastAsync();
            return data.Select(x => new FirstInSecondWeatherForecast
            {
                TemperatureF = x.TemperatureF,
                TemperatureC = x.TemperatureC,
                Date = x.Date,
                Summary = x.Summary
            }).ToArray();
        }
    }
}
