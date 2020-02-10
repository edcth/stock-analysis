using System;
using System.Collections.Generic;
using System.Linq;
using core.Shared;

namespace core.Portfolio.Output
{
    public class TransactionList
    {
        public TransactionList(IEnumerable<Transaction> transactions, string groupBy)
        {
            this.Transactions = Ordered(transactions, groupBy);
            
            if (groupBy != null)
            {
                this.Grouped = Ordered(transactions, groupBy)
                    .GroupBy(t => GroupByValue(groupBy, t))
                    .Select(g => new {
                        name = g.Key,
                        transactions = new TransactionList(g, null)
                    });
            }
        }

        private IEnumerable<Transaction> Ordered(IEnumerable<Transaction> transactions, string groupBy)
        {
            if (groupBy == "ticker")
            {
                return transactions.OrderBy(t => t.Ticker);
            }

            return transactions.OrderByDescending(t => t.Date);
        }

        private static string GroupByValue(string groupBy, Transaction t)
        {
            if (groupBy == "ticker")
            {
                return t.Ticker;
            }
            return t.Date.ToString("yyyy-MM-01");
        }

        public IEnumerable<Transaction> Transactions { get; }
        public IEnumerable<object> Grouped { get; } 
        
        public double Credit => Transactions.Sum(t => t.Credit);
        public double Debit => Transactions.Sum(t => t.Debit);
        public double OptionDebit => Transactions.Where(t => t.IsOption).Sum(t => t.Debit);
        public double OptionCredit => Transactions.Where(t => t.IsOption).Sum(t => t.Credit);
        public double Profit => this.Credit - this.Debit;
    }
}