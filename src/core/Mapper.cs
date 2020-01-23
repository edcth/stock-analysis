using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Adapters.Options;
using core.Adapters.Stocks;
using core.Notes;
using core.Notes.Output;
using core.Options;
using core.Portfolio.Output;
using core.Shared;
using core.Stocks;

namespace core
{
    public class Mapper
    {
        public static OptionDetailsViewModel MapOptionDetails(
            double price,
            IEnumerable<OptionDetail> options)
        {
            var optionList = options
                .Where(o => o.Volume > 0 || o.OpenInterest > 0)
                .Where(o => o.ParsedExpirationDate > DateTime.UtcNow)
                .OrderBy(o => o.ExpirationDate)
                .ToArray();

            var expirations = optionList.Select(o => o.ExpirationDate)
                .Distinct()
                .OrderBy(s => s)
                .ToArray();

            var puts = optionList.Where(o => o.IsPut);
            var calls = optionList.Where(o => o.IsCall);

            return new OptionDetailsViewModel
            {
                StockPrice = price,
                Options = optionList,
                Expirations = expirations,
                LastUpdated = optionList.Max(o => o.LastUpdated),
                Breakdown = new OptionBreakdownViewModel
                {
                    CallVolume = calls.Sum(o => o.Volume),
                    CallSpend = calls.Sum(o => o.Volume * o.Bid),
                    PriceBasedOnCalls = calls.Average(o => o.Volume * o.StrikePrice) / calls.Average(o => o.Volume),

                    PutVolume = puts.Sum(o => o.Volume),
                    PutSpend = puts.Sum(o => o.Volume * o.Bid),
                    PriceBasedOnPuts = puts.Average(o => o.Volume * o.StrikePrice) / puts.Average(o => o.Volume),
                }
            };
        }

        internal static TransactionList ToTransactionLog(
            IEnumerable<OwnedStock> stocks,
            IEnumerable<SoldOption> options,
            string ticker)
        {
            var log = stocks.Where(s => s.State.Ticker == (ticker != null ? ticker : s.State.Ticker))
            .SelectMany(s => s.State.Transactions)
            .Union(options.Where(o => o.State.Ticker == (ticker != null ? ticker : o.State.Ticker))
            .SelectMany(o => o.State.Transactions))
            .OrderByDescending(t => t.Date)
            .ToList();

            return new TransactionList(log);
        }

        internal static NotesList MapNotes(IEnumerable<Note> notes)
        {
            return new NotesList {
                Notes = notes.Select(n => n.State)
            };
        }

        public static object MapLists(List<StockQueryResult> active, List<StockQueryResult> gainers, List<StockQueryResult> losers)
        {
            return new {
                active,
                gainers,
                losers
            };
        }

        public static object MapStockDetail(
            string ticker,
            double price,
            CompanyProfile profile,
            StockAdvancedStats stats,
            HistoricalResponse data,
            MetricsResponse metrics)
        {
            var byMonth = data?.Historical?.GroupBy(r => r.Date.ToString("yyyy-MM-01"))
                .Select(g => new
                {
                    Date = DateTime.Parse(g.Key),
                    Price = g.Average(p => p.Close),
                    Volume = Math.Round(g.Average(p => p.Volume) / 1000000.0, 2),
                    Low = g.Min(p => p.Close),
                    High = g.Max(p => p.Close)
                });

            var labels = byMonth?.Select(a => a.Date.ToString("MMMM"));
            var lowValues = byMonth?.Select(a => Math.Round(a.Low, 2));
            var highValues = byMonth?.Select(a => Math.Round(a.High, 2));

            var priceValues = byMonth?.Select(a => Math.Round(a.Price, 2));
            var priceChartData = labels?.Zip(priceValues, (l, p) => new object[] { l, p });

            var volumeValues = byMonth?.Select(a => a.Volume);
            var volumeChartData = labels?.Zip(volumeValues, (l, p) => new object[] { l, p });

            var metricDates = metrics?.Metrics?.Select(m => m.Date.ToString("MM/yy")).Reverse();

            var bookValues = metrics?.Metrics?.Select(m => m.BookValuePerShare).Reverse();
            var bookChartData = metricDates?.Zip(bookValues, (l, p) => new object[] { l, p });

            var peValues = metrics?.Metrics?.Select(m => m.PERatio).Reverse();
            var peChartData = metricDates?.Zip(peValues, (l, p) => new object[] { l, p });

            return new
            {
                ticker,
                price,
                stats,
                profile,
                labels,
                priceChartData,
                volumeChartData,
                bookChartData,
                peChartData
            };
        }

        public static object ToOptionView(SoldOption o)
        {
            return new
            {
                ticker = o.State.Ticker,
                type = o.State.Type.ToString(),
                strikePrice = o.State.StrikePrice,
                expiration = o.State.Expiration.ToString("yyyy-MM-dd"),
                premium = o.State.Premium,
                amount = o.State.Amount,
                riskPct = o.State.Premium / (o.State.StrikePrice * 100) * 100,
                profit = o.State.Profit
            };
        }

        internal static object ToOwnedView(OwnedStock o)
        {
            return new
            {
                ticker = o.State.Ticker,
                owned = o.State.Owned,
                spent = Math.Round(o.State.Spent, 2),
                earned = Math.Round(o.State.Earned, 2)
            };
        }
    }
}