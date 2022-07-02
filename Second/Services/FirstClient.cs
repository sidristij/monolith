using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Second.Services
{
    public class FirstClient : IFirstClient
    {
        private readonly IOptions<FirstConfiguration> _options;

        public FirstClient(IOptions<FirstConfiguration> options)
        {
            _options = options;
        }

        public Task<FirstInSecondWeatherForecast[]> GetForecastAsync()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(_options.Value.Url)
            };

            return client.GetFromJsonAsync<FirstInSecondWeatherForecast[]>("WeatherForecast");
        }
    }
}
