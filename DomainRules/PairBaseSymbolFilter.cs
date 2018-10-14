using Infrastructure;
using System;
using System.Linq.Expressions;

namespace DomainModels.Filters
{
    public sealed class PairBaseSymbolFilter : Specification<Pair>
    {
        private string BaseSymbol { get; }

        public PairBaseSymbolFilter(string baseSymbol)
            => (BaseSymbol) = (baseSymbol);

        public override Expression<Func<Pair, bool>> ToExpression()
            => pair => pair.BaseSymbol.Equals(BaseSymbol, StringComparison.InvariantCultureIgnoreCase);
    }
}
