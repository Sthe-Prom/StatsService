namespace StatsService.Interfaces
{
    public interface IBitlyClickCountService
    {
        Task<int> GetClickCountForBitlink(string bitlinkUrl);
    }
}