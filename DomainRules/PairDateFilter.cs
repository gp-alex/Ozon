using Infrastructure;
using System;
using System.Linq.Expressions;

namespace DomainModels.Filters
{
    public sealed class PairDateFilter : Specification<Pair>
    {
        private int Year { get; }
        private int Month { get; }

        public PairDateFilter(int month, int year)
            => (Year, Month) = (year, month);

        public override Expression<Func<Pair, bool>> ToExpression()
            => pair => pair.Dt.Year == Year && pair.Dt.Month == Month;
    }
}
