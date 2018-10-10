using System;

namespace DomainModels
{
    public class Pair
    {
        public DateTime Dt { get; set; }
        public string BaseSymbol { get; set; }
        public string CounterSymbol { get; set; }
        public double Rate { get; set; }
    }
}
