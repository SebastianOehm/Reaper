namespace Reaper
{
    public static class JsonResponseDeserializer1
    {
        public class weather
        {
            public string description { get; set; }
            //public string main { get; set; } //not used
            //public string icon { get; set; } //not used in console
        }
        public class main
        {
            public double temp { get; set; }
            public double temp_min { get; set; }
            public double temp_max { get; set; }
            public double feels_like { get; set; }
            public int pressure { get; set; }
            public int humidity { get; set; }
        }
        public class sys
        {
            public long sunrise { get; set; }
            public long sunset { get; set; }
            public string country { get; set; }
        }
        public class wind
        {
            public double speed { get; set; }
            public int deg { get; set; }
        }

        public class root
        {
            public List<weather> weather { get; set; }
            public main main { get; set; }
            public sys sys { get; set; }
            public wind wind { get; set; }
            public long dt { get; set; }
            public int timezone { get; set; }
            public string name { get; set; }
        }

    }
}