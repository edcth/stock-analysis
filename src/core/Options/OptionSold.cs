﻿using System;
using core.Shared;

namespace core.Options
{
    public class OptionSold : AggregateEvent
    {
        public OptionSold(
            Guid id,
            Guid aggregateId,
            DateTimeOffset when,
            int amount,
            double premium)
            : base(id, aggregateId, when)
        {
            this.Amount = amount;
            this.Premium = premium;
        }

        public int Amount { get; }
        public double Premium { get; }
    }
}