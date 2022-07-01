using System.Threading.Tasks;

namespace First.Services
{
    public interface IFirstWeatherService
    {
        Task<FirstWeatherForecast[]> GetForecastAsync();
    }
}
