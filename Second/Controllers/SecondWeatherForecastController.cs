using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Second.Services;

namespace Second.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class SecondWeatherForecastController : ControllerBase
	{
        private readonly IFirstClient _firstClient;

        private static readonly string[] Summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};


		public SecondWeatherForecastController(IFirstClient firstClient)
        {
            _firstClient = firstClient;
        }

		[HttpGet]
		public IEnumerable<SecondWeatherForecast> GetSecond()
		{
			var rng = new Random();
			return Enumerable.Range(1, 5).Select(index => new SecondWeatherForecast
				{
					Date = DateTime.Now.AddDays(index),
					TemperatureC = rng.Next(-20, 55),
					Summary = Summaries[rng.Next(Summaries.Length)]
				})
				.ToArray();
		}

        [HttpGet("first")]
        public Task<FirstInSecondWeatherForecast[]> GetFirst()
        {
            return _firstClient.GetForecastAsync();
        }
    }
}