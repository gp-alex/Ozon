using DomainModels;
using Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainServices
{
    public class WeeklyReportBuilder
    {
        private DateTime Dt { get; set; }
        private string BaseCurrency { get; set; }
        private string CounterCurrency { get; set; }
        private IEnumerable<Pair> Pairs { get; set; }

        public WeeklyReportBuilder SetDate(DateTime dt)
        {
            this.Dt = dt;
            return this;
        }

        public WeeklyReportBuilder SetBaseCurrency(string baseCurrency)
        {
            BaseCurrency = !string.IsNullOrEmpty(baseCurrency) ? baseCurrency : throw new ArgumentNullException(nameof(baseCurrency));
            return this;
        }

        public WeeklyReportBuilder SetCounterCurrency(string counterCurrency)
        {
            CounterCurrency = !string.IsNullOrEmpty(counterCurrency) ? counterCurrency : throw new ArgumentNullException(nameof(counterCurrency));
            return this;
        }

        public WeeklyReportBuilder SetData(IEnumerable<Pair> pairs)
        {
            this.Pairs = pairs ?? throw new ArgumentNullException(nameof(pairs));
            return this;
        }

        public ForexWeeklyReport Build()
        {
            if (string.IsNullOrEmpty(BaseCurrency)) throw new ArgumentNullException(nameof(BaseCurrency));
            if (string.IsNullOrEmpty(CounterCurrency)) throw new ArgumentNullException(nameof(CounterCurrency));
            if (Pairs == null) throw new ArgumentNullException(nameof(Pairs));

            int numberOfPairs = Pairs.Count();
            double median = Pairs
                    .OrderBy(x => x.Rate)
                    .Select((pair, index) => new { index, pair })
                    .Where(x => x.index == numberOfPairs / 2)
                    .Select(x => x.pair.Rate)
                    .SingleOrDefault();

            return new ForexWeeklyReport(
                Dt,
                DateTimeExtensions.LastBusinessDtOfWeek(Dt),
                BaseCurrency,
                CounterCurrency,
                Pairs.Select(x => x.Rate).Min(),
                Pairs.Select(x => x.Rate).Max(),
                median
            );
        }
    }
}
