namespace PedaloWebApp.Pages.Bookings
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Newtonsoft.Json;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using NuGet.Configuration;
    using System.Buffers.Text;
    using System;

    public class WeatherModel : PageModel
    {
        private readonly IHttpClientFactory httpClientFactory;


        public WeatherModel(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

                
        public WeatherData weatherCode { get; set; }

        public string WeatherDescription { get; set; }

        public async Task OnGetAsync()
        {
            var client = httpClientFactory.CreateClient();

            string apiUrl = "https://api.open-meteo.com/v1/forecast?latitude=47.37&longitude=8.55&daily=weathercode,temperature_2m_max,temperature_2m_min&forecast_days=5&timezone=auto";

            var response = await client.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                //JObject root = JObject.Parse(content);
                //weatherCode = root[Wea]
                weatherCode = JsonConvert.DeserializeObject<WeatherData>(content);
                //WeatherCodes = WeatherData.Daily.Select(d => d.Weather[0].WeatherCode).ToArray();
                
            }

        }
    }

    public class WeatherData
    {
        public Daily daily { get; set; }
        //public WeatherCode[] Daily { get; set; }
    }

    public class Daily
    {
        public DateTime[] time { get; set; }
        public int[] weathercode { get; set; }
        public double[] temperature_2m_max { get; set; }
        public double[] temperature_2m_min { get; set; }
    }

    public class WeatherCode
    {
        public int weatherCode { get; set; }
        public float Temperature2mMax { get; set; }
        public float Temperature2mMin { get; set; }
    }

}
