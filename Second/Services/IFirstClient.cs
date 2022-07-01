using System.Threading.Tasks;

namespace Second.Services
{
    public interface IFirstClient
    {
        Task<FirstInSecondWeatherForecast[]> GetForecastAsync();
    }
}
