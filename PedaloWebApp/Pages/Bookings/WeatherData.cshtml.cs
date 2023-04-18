namespace PedaloWebApp.Pages.Bookings
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Newtonsoft.Json;
    using System.Net.Http;
    using System.Threading.Tasks;



    public class WeatherModel : PageModel
    {
        private readonly IHttpClientFactory httpClientFactory;



        public WeatherModel(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }



        public WeatherData WeatherData { get; set; }



        public async Task OnGetAsync()
        {
            var client = httpClientFactory.CreateClient();



            string apiUrl = "https://api.open-meteo.com/v1/forecast?latitude=47.37&longitude=8.55&daily=weathercode,temperature_2m_max,temperature_2m_min&forecast_days=3&timezone=auto";



            var response = await client.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                WeatherData = JsonConvert.DeserializeObject<WeatherData>(content);
            }
        }
    }

    public class WeatherData
    {
        public WeatherCode[] Daily { get; set; }
    }



    public class WeatherCode
    {
        public int weatherCode { get; set; }
        public float Temperature2mMax { get; set; }
        public float Temperature2mMin { get; set; }
    }

}
