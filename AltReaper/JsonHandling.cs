using System.Text.Json.Serialization;

namespace Reaper
{
    internal static class JsonHandling
    {
        public class Config
        {
            [JsonPropertyName("apiKey")]
            public string ApiKey { get; set; }
            [JsonPropertyName("senderMail")]
            public string SenderMail { get; set; }

            [JsonPropertyName("senderMailPassword")]
            public string SenderMailPassword { get; set; }
            [JsonPropertyName("hostDomain")]
            public string HostDomain { get; set; }
            [JsonPropertyName("portNumber")]
            public string PortNumber { get; set; }
            [JsonPropertyName("bcc")]
            public string Bcc { get; set; }
        }
    }
    public static class WeatherResponse
    {
        public class Weather
        {
            [JsonPropertyName("description")]
            public string Description { get; set; }
            [JsonPropertyName("main")]
            public string Main { get; set; }
            [JsonPropertyName("icon")]
            public string Icon { get; set; }
        }
        public class Main
        {
            [JsonPropertyName("temp")]
            public double Temp { get; set; }
            [JsonPropertyName("temp_min")]
            public double TempMin { get; set; }
            [JsonPropertyName("temp_max")]
            public double TempMax { get; set; }
            [JsonPropertyName("feels_like")]
            public double FeelsLike { get; set; }
            [JsonPropertyName("pressure")]
            public int Pressure { get; set; }
            [JsonPropertyName("humidity")]
            public int Humidity { get; set; }
        }
        public class Sys
        {
            [JsonPropertyName("sunrise")]
            public long Sunrise { get; set; }
            [JsonPropertyName("sunset")]
            public long Sunset { get; set; }
            [JsonPropertyName("country")]
            public string Country { get; set; }
        }
        public class Wind
        {
            [JsonPropertyName("speed")]
            public double Speed { get; set; }
            [JsonPropertyName("deg")]
            public int Deg { get; set; }
        }

        public class Root
        {
            [JsonPropertyName("weather")]
            public List<Weather> Weather { get; set; }
            [JsonPropertyName("main")]
            public Main Main { get; set; }
            [JsonPropertyName("sys")]
            public Sys Sys { get; set; }
            [JsonPropertyName("wind")]
            public Wind Wind { get; set; }
            [JsonPropertyName("dt")]
            public long Dt { get; set; }
            [JsonPropertyName("timezone")]
            public int Timezone { get; set; }
            [JsonPropertyName("name")]
            public string Name { get; set; }
        }
    }
}