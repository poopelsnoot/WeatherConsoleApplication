using Assignment_A1_01.Models;
using Assignment_A1_01.Services;

namespace Assignment_A1_01
{
    class Program
    {
        static async Task Main(string[] args)
        {
            double latitude = 59.5086798659495;
            double longitude = 18.2654625932976;

            Forecast forecast = await new OpenWeatherService().GetForecastAsync(latitude, longitude);
            

            //present each forecast item in a grouped list
            Console.WriteLine($"Weather forecast for {forecast.City}");
            var groupDate = forecast.Items.GroupBy(f => f.DateTime.Date, forecastItem => forecastItem);
            foreach(var group in groupDate)
            {
                await Console.Out.WriteLineAsync($"{group.Key.ToString("yyyy-MM-dd")}");
                group.ToList().ForEach(t => Console.WriteLine(t));
            }
        }
    }
}