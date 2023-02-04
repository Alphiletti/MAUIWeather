using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUIWeather.MVVM.Models;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MAUIWeather.MVVM.ViewModels
{

    [AddINotifyPropertyChangedInterface]
    public partial class WeatherViewModel
    {
        public WeatherData weatherData { get; set; }
        public string PlaceName { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        private HttpClient client;

        public WeatherViewModel()
        {
            client = new HttpClient();
        }

        public bool IsVisible { get; set; }
        public bool IsLoading { get; set; }

        [RelayCommand]
        public async void Search(object searchText)
        {
            PlaceName = searchText.ToString();
            var location = await GetCoordinatesAsync(searchText.ToString());

            await GetWeather(location);
        }

        private async Task GetWeather(Location location)
        {
            var url = $"https://api.open-meteo.com/v1/forecast?latitude={location.Latitude}&longitude={location.Longitude}&daily=weathercode,temperature_2m_max,temperature_2m_min&current_weather=true&timezone=auto";
            IsLoading = true;
            var response = await client.GetAsync(url);

            if(response.IsSuccessStatusCode)
            {
                using(var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    var data = await JsonSerializer.DeserializeAsync<WeatherData>(responseStream);
                    weatherData = data;
                    for(int i=0; i < weatherData.daily.time.Length; i++)
                    {
                        var daily2 = new Daily2
                        {
                            time = weatherData.daily.time[i],
                            temperature_2m_max = weatherData.daily.temperature_2m_max[i],
                            temperature_2m_min = weatherData.daily.temperature_2m_min[i],
                            weathercode = weatherData.daily.weathercode[i]
                        };
                        weatherData.daily2.Add(daily2);
                    }
                    IsVisible = true;
                }
            }
            IsLoading = false;
        }

        private async Task<Location> GetCoordinatesAsync(string address)
        {
            IEnumerable<Location> locations = await Geocoding.Default.GetLocationsAsync(address);
            Location location = locations?.FirstOrDefault();

            if(location != null)
            {
                Console.WriteLine($"Latitude: {location.Latitude}");
            }
            return location;
        }
    }
}
