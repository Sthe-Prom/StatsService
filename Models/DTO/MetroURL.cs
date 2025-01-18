using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StatsService.Models.DTO
{
    public class MetroURL
    {
        [Key]
        public int Id { get; set; }
        public string LongUrl { get; set; }
        public string ShortUrl { get; set; }
        public int ClicksCount { get; set; } = 0;
    }
}