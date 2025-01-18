using StatsService.Services;
using StatsService.Interfaces;
using StatsService.Models;
using StatsService.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace StatsService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class URLStatsController: ControllerBase
    {
        private readonly URLStatsService _statsService;
        private readonly IBitlyClickCountService _bitlyService;

        public URLStatsController(URLStatsService statsService, IBitlyClickCountService bitlyService)
        {
            _statsService = statsService;
            _bitlyService = bitlyService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllMetroURLs()
        {
            //Call service method or directly make the request with HttpClient
            IEnumerable<MetroURL> MetroURLs;

            if (_statsService != null)
            {
                MetroURLs = await _statsService.GetURLsAsync();
            }
            else
            {
                //Use HttpClient directly if not using service
                var client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:5240/metrourl/"); // Replace with base URL
                var response = await client.GetAsync("all");
                response.EnsureSuccessStatusCode();
                MetroURLs = await response.Content.ReadFromJsonAsync<IEnumerable<MetroURL>>();
            }
            
            return Ok(MetroURLs);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddUrl(string longUrl)
        {
            try
            {
                var response = await _statsService.AddMetroURL(longUrl);

                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the response to get the ID of the Shortened URL/Newly created URLs
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var model = JsonConvert.DeserializeObject<MetroURL>(responseContent);

                    // Construct the URL for the newly created resource
                    string redirectUrl = model.ShortUrl;

                    //return Redirect(redirectUrl);

                    return StatusCode((int)response.StatusCode, $"Url Edded Successfully: {await response.Content.ReadAsStringAsync()}");
                }
                else
                {
                    // Handle unsuccessful responses 
                    return StatusCode((int)response.StatusCode, $"Error adding Metro URL: {await response.Content.ReadAsStringAsync()}");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error, return an error response)
                return StatusCode(500, "Error adding Metro URL");
            }
        }
                
        [HttpGet("id")]
        public async Task<IActionResult> GetMetroUrlById(int id)
        {
            try
            {
                var metroUrl = await _statsService.GetMetroUrlById(id);

                if (metroUrl != null)
                {
                    return Ok(metroUrl);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., using a logging framework)
                return StatusCode(500, "An error occurred while retrieving the MetroURL.");
            }
        }

        [HttpPut("edit")]
        public async Task<IActionResult> UpdateClicks(int id, MetroURL model)
        {
            try
            {
                var metroUrl = await _statsService.UpdateClicks(id, model);

                if (metroUrl != null)
                {
                    return Ok(metroUrl);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., using a logging framework)
                return StatusCode(500, "An error occurred while retrieving the MetroURL.");
            }
        }

        //Do Click Stats (Dashboard) //Opening the dashboard will run fetch update from the bitly API to update latest click numbers.
        [HttpGet("dashboard")]
        public async Task<IActionResult> UpdateStats()
        {
            //Call service method or directly make the request with HttpClient
            IEnumerable<MetroURL> MetroURLs;

            if (_statsService != null)
            {
                MetroURLs = await _statsService.GetURLsAsync();
            }
            else
            {
                //Use HttpClient directly if not using service
                var client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:5240/metrourl/"); 
                var response = await client.GetAsync("all");
                response.EnsureSuccessStatusCode();
                MetroURLs = await response.Content.ReadFromJsonAsync<IEnumerable<MetroURL>>();
            }

            foreach (var item in MetroURLs)
            {
                var click_count = _bitlyService.GetClickCountForBitlink(item.ShortUrl);
                UpdateClicks(item.Id, item);
            }
        
            return Ok(MetroURLs);
        }
    }
}