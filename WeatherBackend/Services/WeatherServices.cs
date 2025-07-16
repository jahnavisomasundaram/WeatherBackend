using System.Net.Http;
using System.Text.Json;
using WeatherBackend.Models;

public class WeatherService
{
    private readonly HttpClient _http;
    private readonly string _apiKey = "c06d0be622563127c49524eb461ced09";
    private readonly string _baseUrl = "https://api.openweathermap.org/data/2.5/weather";


    public WeatherService(HttpClient http)
    {
        _http = http;
    }

    public async Task<WeatherResponse?> GetWeatherAsync(string city)
    {
        var url = $"{_baseUrl}?q={city}&appid={_apiKey}&units=metric";
        return await _http.GetFromJsonAsync<WeatherResponse>(url);
    }
}