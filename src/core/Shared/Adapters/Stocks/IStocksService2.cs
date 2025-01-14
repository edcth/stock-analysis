using System.Collections.Generic;
using System.Threading.Tasks;

namespace core.Adapters.Stocks
{
    public interface IStocksService2
    {
        Task<CompanyProfile> GetCompanyProfile(string ticker);
        Task<StockAdvancedStats> GetAdvancedStats(string ticker);
        Task<TickerPrice> GetPrice(string ticker);
        Task<Dictionary<string, BatchStockPrice>> GetPrices(List<string> tickers);
        
        Task<List<SearchResult>> Search(string fragment);
        Task<Quote> Quote(string ticker);
    }
}