using System.Collections.Generic;
using System.Threading.Tasks;
using First.Services;
using Microsoft.AspNetCore.Mvc;

namespace First.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class WeatherForecastController : ControllerBase
	{
        private readonly IFirstWeatherService _weatherService;

        public WeatherForecastController(IFirstWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

		[HttpGet]
		public async Task<IEnumerable<FirstWeatherForecast>> Get()
        {
            return await _weatherService.GetForecastAsync();
        }
	}
}