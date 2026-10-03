using System.Text.Json.Serialization;

namespace Reaper
{
    internal static class JsonHandling
    {
        public class Config
        {
            [JsonPropertyName("apiKey")]
            public string ApiKey { get; set; } = string.Empty;
            [JsonPropertyName("senderMail")]
            public string SenderMail { get; set; } = string.Empty;

            [JsonPropertyName("senderMailPassword")]
            public string SenderMailPassword { get; set; } = string.Empty;
            [JsonPropertyName("hostDomain")]
            public string HostDomain { get; set; } = string.Empty;
            [JsonPropertyName("portNumber")]
            public string PortNumber { get; set; } = string.Empty;
            [JsonPropertyName("bcc")]
            public string Bcc { get; set; } = string.Empty;
        }
    }
    public static class WeatherResponse
    {
        public class Weather
        {
            [JsonPropertyName("description")]
            public string Description { get; set; } = string.Empty;
            [JsonPropertyName("main")]
            public string Main { get; set; } = string.Empty;
            [JsonPropertyName("icon")]
            public string Icon { get; set; } = string.Empty;
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
            public string Country { get; set; } = string.Empty;
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
            public List<Weather> Weather { get; set; } = [];
            [JsonPropertyName("main")]
            public Main Main { get; set; } = new();
            [JsonPropertyName("sys")]
            public Sys Sys { get; set; } = new();
            [JsonPropertyName("wind")]
            public Wind Wind { get; set; } = new();
            [JsonPropertyName("dt")]
            public long Dt { get; set; }
            [JsonPropertyName("timezone")]
            public int Timezone { get; set; }
            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;
        }
    }
}