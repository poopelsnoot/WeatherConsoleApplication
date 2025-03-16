using System.Net.Http.Json; 
using System.Collections.Concurrent;
using Assignment_A1_03.Models;

namespace Assignment_A1_03.Services
{
    public class OpenWeatherService
    {
        HttpClient httpClient = new HttpClient();
        
        //Cache declaration
        ConcurrentDictionary<(double, double, string), Forecast> cachedGeoForecasts = new ConcurrentDictionary<(double, double, string), Forecast>();
        ConcurrentDictionary<(string, string), Forecast> cachedCityForecasts = new ConcurrentDictionary<(string, string), Forecast>();

        // API Key
        readonly string apiKey = "1da5e245154c235707786bf7531a8b25";

        //Event declaration
        public event EventHandler<string> WeatherForecastAvailable;
        public event EventHandler<string> CachedForecastAvailable;

        protected virtual void OnWeatherForecastAvailable (string message)
        {
            WeatherForecastAvailable?.Invoke(this, message);
        }
        protected virtual void OnCachedForecastAvailable(string message)
        {
            CachedForecastAvailable?.Invoke(this, message);
        }
        public async Task<Forecast> GetForecastAsync(string City)
        {
            //check if forecast in Cache
            //generates an event that shows forecast was from cache
            if (cachedCityForecasts.ContainsKey((City, DateTime.Now.ToString("yyyy-MM-dd HH:mm"))))
            {
                Forecast cachedForecast = cachedCityForecasts[(City, DateTime.Now.ToString("yyyy-MM-dd HH:mm"))];

                OnCachedForecastAvailable(City);
                return cachedForecast;
            }

            //https://openweathermap.org/current
            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var uri = $"https://api.openweathermap.org/data/2.5/forecast?q={City}&units=metric&lang={language}&appid={apiKey}";

            Forecast forecast = await ReadWebApiAsync(uri);

            //generates an event with different message if cached data
            cachedCityForecasts.TryAdd((City, DateTime.Now.ToString("yyyy-MM-dd HH:mm")), forecast);

            OnWeatherForecastAvailable(City);
            return forecast;

        }
        public async Task<Forecast> GetForecastAsync(double latitude, double longitude)
        {
            //check if forecast in Cache
            //generate an event that shows forecast was from cache
            if (cachedGeoForecasts.ContainsKey((latitude, longitude, DateTime.Now.ToString("yyyy-MM-dd HH:mm"))))
            {
                Forecast cachedForecast = cachedGeoForecasts[(latitude, longitude, DateTime.Now.ToString("yyyy-MM-dd HH:mm"))];

                OnCachedForecastAvailable($"({latitude}, {longitude})");
                return cachedForecast;
            }

            //https://openweathermap.org/current
            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var uri = $"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&units=metric&lang={language}&appid={apiKey}";

            Forecast forecast = await ReadWebApiAsync(uri);

            //part of event and cache code
            //generates an event with different message if cached data
            cachedGeoForecasts.TryAdd((latitude, longitude, DateTime.Now.ToString("yyyy-MM-dd HH:mm")), forecast);

            OnWeatherForecastAvailable($"({latitude}, {longitude})");
            return forecast;
        }
        private async Task<Forecast> ReadWebApiAsync(string uri)
        {
            //Read the response from the WebApi
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            WeatherApiData wd = await response.Content.ReadFromJsonAsync<WeatherApiData>();


            //Converts WeatherApiData to Forecast using Linq.
            Forecast forecast = new Forecast();
            forecast.City = wd.city.name;
            forecast.Items = wd.list.Select( w => new ForecastItem
            {
                DateTime = UnixTimeStampToDateTime(w.dt),
                Temperature = w.main.temp,
                WindSpeed = w.wind.speed,
                Description = w.weather[0].description
            }).ToList();

            return forecast;
        }
        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
    }
}