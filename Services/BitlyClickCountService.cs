using StatsService.Services;
using StatsService.Interfaces;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;

namespace StatsService.Services
{
    public class BitlyClickCountService: IBitlyClickCountService
    {
        private readonly HttpClient _httpClient;
        private readonly string _bitlyAccessToken; 

        public BitlyClickCountService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _bitlyAccessToken = "35cb00286c4d7d22be87eb1ffd983557894c8f88";
        }

        public async Task<int> GetClickCountForBitlink(string bitlinkUrl)
        {
            // Replace with the actual Bitly API endpoint for retrieving click counts
            string url = $"https://api-ssl.bitly.com/v4/bitlinks/{bitlinkUrl}/clicks";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _bitlyAccessToken);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();


                // Deserialize the JSON response
                var clickData = System.Text.Json.JsonSerializer.Deserialize<BitlyClickData>(responseContent, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                            
                int clickCount = clickData.Units; // Parse click count from responseContent

                //get clicks
                int totalClicks = 0;
                foreach (var click in clickData.LinkClicks)
                {
                    totalClicks += click.Clicks;
                }
               
                return clickCount;
            }
            else
            {
                throw new Exception($"Failed to retrieve click count for {bitlinkUrl}: {await response.Content.ReadAsStringAsync()}");
            }
        }


        public class BitlyClickData
        {
            public List<Click> LinkClicks { get; set; }
            public int Units { get; set; }
            public string Unit { get; set; }
            public string UnitReference { get; set; }

            public class Click
            {
                public int Clicks { get; set; }
                public string Date { get; set; }
            }
        }

    }
}