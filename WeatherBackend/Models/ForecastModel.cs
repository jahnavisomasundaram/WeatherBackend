namespace WeatherBackend.Models
{
    public class ForecastModel
    {
        public class ForecastItem
        {
            public Main main { get; set; }
            public List<Weather> weather { get; set; }
            public string dt_txt { get; set; }
        }

        public class Main
        {
            public float temp { get; set; }
        }

        public class Weather
        {
            public string description { get; set; }
            public string icon { get; set; }
        }

        public class ForecastResponse
        {
            public List<ForecastItem> list { get; set; }
        }

    }
}
