using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceShopper.Domain.ValueObjects
{
    public sealed class Money
    {
        public decimal Amount { get; }
        public string Currency { get; }
        private Money() { }
        public Money(decimal price, string Currency) 
        { 
            this.Amount = price;
            this.Currency = Currency;
        }
    }
}
