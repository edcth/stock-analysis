using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace core.Stocks
{
    public class Sell
    {
        public class Command : StockTransaction {}

        public class Handler : HandlerWithStorage<Command, Unit>
        {
            public Handler(IPortfolioStorage storage) : base(storage)
            {
            }

            public override async Task<Unit> Handle(Command cmd, CancellationToken cancellationToken)
            {
                var stock = await this._storage.GetStock(cmd.Ticker, cmd.UserId);

                if (stock == null)
                {
                    return new Unit();
                }

                stock.Sell(cmd.NumberOfShares, cmd.Price, cmd.Date.Value);

                await this._storage.Save(stock, cmd.UserId);

                return new Unit();
            }
        }
    }
}