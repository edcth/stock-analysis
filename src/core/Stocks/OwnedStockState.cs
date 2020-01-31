using System;
using System.Collections.Generic;
using core.Shared;

namespace core.Stocks
{
    public class OwnedStockState
	{
        public OwnedStockState()
        {
            this.Transactions = new List<Transaction>();
            this.Buys = new List<StockPurchased>();
            this.Sells = new List<StockSold>();
        }

		public string Ticker { get; internal set; }
		public string UserId { get; internal set; }
		public int Owned { get; internal set; }
		public double Spent { get; internal set; }
		public double Earned { get; internal set; }
        public DateTimeOffset Purchased { get; internal set; }
        public DateTimeOffset? Sold { get; internal set; }
        public double Profit => this.Sold != null ? this.Earned - this.Spent : 0;

        public List<Transaction> Transactions { get; private set; }
        public double Cost => Math.Round(Math.Abs(this.Spent - this.Earned), 2);

        public Guid Id { get; set; }
        internal List<StockPurchased> Buys { get; }
        internal List<StockSold> Sells { get; }

        internal void Apply(StockPurchased purchased)
        {
            Owned += purchased.NumberOfShares;
            Spent += purchased.NumberOfShares * purchased.Price;
            Purchased = purchased.When;

            this.Buys.Add(purchased);

            this.Transactions.Add(Transaction.DebitTx(
                this.Ticker,
                $"Purchased {purchased.NumberOfShares} shares @ ${purchased.Price}/share",
                purchased.Price * purchased.NumberOfShares,
                purchased.When
            ));
        }

        internal void Apply(StockSold sold)
        {
            this.Owned -= sold.NumberOfShares;
            this.Earned += sold.NumberOfShares * sold.Price;
            this.Sold = sold.When;

            this.Sells.Add(sold);
            
            this.Transactions.Add(Transaction.CreditTx(
                this.Ticker,
                $"Sold {sold.NumberOfShares} shares @ ${sold.Price}/share",
                sold.Price * sold.NumberOfShares,
                sold.When
            ));
        }
    }
}