using StatsService.Services;
using StatsService.Models;
using StatsService.Models.DTO;
using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace StatsService.Services
{
    public class URLStatsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public URLStatsService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("URLStatsService"); // Name of the HttpClient in your configuration
           // _baseUrl = baseUrl;
        }

        //All URLs
        public async Task<IEnumerable<MetroURL>> GetURLsAsync()
        {
            var response = await _httpClient.GetAsync("metrourl/all");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<MetroURL>>();
        }

        //Shorten Url
        public async Task<HttpResponseMessage> AddMetroURL(string longUrl)
        {
            string url = $"http://localhost:5240/MetroURL/add?LongUrl={longUrl}";
            var response = await _httpClient.PostAsync(url, null);

            string responseContent = await response.Content.ReadAsStringAsync();
            var metroUrl = Newtonsoft.Json.JsonConvert.DeserializeObject<MetroURL>(responseContent);

            return response;            

        }

        //Update (Count) Clicks
        public async Task<HttpResponseMessage> UpdateClicks(int id, MetroURL model)
        {
            string url = $"http://localhost:5240/MetroURL/id?Id={id}";
            
            var jsonContent = JsonSerializer.Serialize(model, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(url, stringContent);
            return response;

        }

        public async Task<MetroURL> GetMetroUrlById(int id)
        {
            try
            {
                string url = $"http://localhost:5240/MetroURL/id?Id={id}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var metroUrl = Newtonsoft.Json.JsonConvert.DeserializeObject<MetroURL>(responseContent);
                    return metroUrl;
                }
                else
                {
                    // Handle unsuccessful responses (e.g., log the error)
                    Console.WriteLine($"Error getting MetroURL by ID: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error)
                Console.WriteLine($"Error getting MetroURL by ID: {ex.Message}");
                return null;
            }
        }




    }
}