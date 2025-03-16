using System;

namespace Assignment_A1_03.Models
{
    public class ForecastItem
    {
        public DateTime DateTime { get; set; }
        public double Temperature { get; set; }
        public double WindSpeed { get; set; }
        public string Description { get; set; }
        public override string ToString()
        {
            return $"   - {DateTime.ToString("HH:mm")}: {Description}, temperature: {Temperature} degC, wind: {WindSpeed} m/s";
        }
    }
}
