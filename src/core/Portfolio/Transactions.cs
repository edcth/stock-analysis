using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using core.Shared;

namespace core.Portfolio
{
    public class Transactions
    {
        public class Query : RequestWithUserId<IEnumerable<Transaction>>
        {
            public Query(string userId, string ticker) : base(userId)
            {
                this.Ticker = ticker;
            }

            public string Ticker { get; }
        }

        public class Handler : HandlerWithStorage<Query, IEnumerable<Transaction>>
        {
            public Handler(IPortfolioStorage storage) : base(storage)
            {
            }

            public override async Task<IEnumerable<Transaction>> Handle(Query request, CancellationToken cancellationToken)
            {
                var stocks = _storage.GetStocks(request.UserId);
                var options = _storage.GetSoldOptions(request.UserId);

                await Task.WhenAll(stocks, options);

                return Mapper.ToTransactionLog(stocks.Result, options.Result, request.Ticker);
            }
        }
    }
}