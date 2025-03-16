using Assignment_A1_02.Models;
using Assignment_A1_02.Services;

namespace Assignment_A1_02
{
    class Program
    {
        static async Task Main(string[] args)
        {
            OpenWeatherService service = new OpenWeatherService();

            //Register the event
            service.WeatherForecastAvailable += EventMessage;

            Task<Forecast>[] tasks = { null, null };
            Exception exception = null;
            try
            {
                double latitude = 59.5086798659495;
                double longitude = 18.2654625932976;

                //Create the two tasks and wait for comletion
                tasks[0] = service.GetForecastAsync(latitude, longitude);
                tasks[1] = service.GetForecastAsync("Gävle");

                Task.WaitAll(tasks[0], tasks[1]);
            }
            catch (Exception ex)
            {
                exception = ex;
                //handle an exception
                ErrorMessage(ex.Message);
            }
            finally
            {
                foreach (var task in tasks)
                {
                    //deal with successful and fault tasks
                    if (task.IsCompletedSuccessfully) PrintForecast(task.Result);
                }
            }
        }

        //Event handler declaration
        static void EventMessage(object sender, string message)
        {
            Console.WriteLine($"Event message from weather service: New weather forecast for {message} available");
        }
        static void ErrorMessage(string message)
        {
            Console.WriteLine("----------------");
            Console.WriteLine("City weather service error");
            Console.WriteLine(message);
        }
        static void PrintForecast(Forecast forecast)
        {
            //present each forecast item in a grouped list
            Console.WriteLine("----------------");
            Console.WriteLine($"Weather forecast for {forecast.City}");
            var groupDate = forecast.Items.GroupBy(f => f.DateTime.Date, forecastItem => forecastItem);
            foreach (var group in groupDate)
            {
                Console.WriteLine($"{group.Key.ToString("yyyy-MM-dd")}");
                group.ToList().ForEach(t => Console.WriteLine(t));
            }
        }
    }
}