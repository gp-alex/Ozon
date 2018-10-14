using DomainModels;
using Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainServices
{
    public class MonthlyReportBuilder
    {
        private int Year { get; set; }
        private int Month { get; set; }
        private string BaseCurrency { get; set; }
        private string CounterCurrency { get; set; }
        private IEnumerable<Pair> Pairs { get; set; }
        private WeeklyReportBuilder WeeklyBuilder { get; }
        public MonthlyReportBuilder(WeeklyReportBuilder weeklyBuilder)
        {
            WeeklyBuilder = weeklyBuilder ?? throw new ArgumentNullException(nameof(weeklyBuilder));
        }

        public MonthlyReportBuilder SetYear(int year)
        {
            Year = year;
            return this;
        }

        public MonthlyReportBuilder SetMonth(int month)
        {
            Month = month;
            return this;
        }

        public MonthlyReportBuilder SetBaseCurrency(string baseCurrency)
        {
            BaseCurrency = !string.IsNullOrEmpty(baseCurrency) ? baseCurrency : throw new ArgumentNullException(nameof(baseCurrency));
            return this;
        }

        public MonthlyReportBuilder SetCounterCurrency(string counterCurrency)
        {
            CounterCurrency = !string.IsNullOrEmpty(counterCurrency) ? counterCurrency  : throw new ArgumentNullException(nameof(counterCurrency));
            return this;
        }

        public MonthlyReportBuilder SetData(IEnumerable<Pair> pairs)
        {
            Pairs = pairs ?? throw new ArgumentNullException(nameof(pairs));
            return this;
        }

        public ForexMonthlyReport Build()
        {
            if (string.IsNullOrEmpty(BaseCurrency)) throw new ArgumentNullException(nameof(BaseCurrency));
            if (string.IsNullOrEmpty(CounterCurrency)) throw new ArgumentNullException(nameof(CounterCurrency));
            if (Pairs == null) throw new ArgumentNullException(nameof(Pairs));

            Pairs = Pairs
                .Where(
                    x => x.BaseSymbol.Equals(BaseCurrency, StringComparison.InvariantCultureIgnoreCase)
                        && x.CounterSymbol.Equals(CounterCurrency, StringComparison.InvariantCultureIgnoreCase)
                )
                .ToList();
            /*var reportStartDate = new DateTime(Year, Month, 1);
            var reportEndDate = reportStartDate.AddDays(DateTime.DaysInMonth(Year, Month) - 1);
            var weeklyReports = Pairs
                .Where(
                    x => x.Dt >= reportStartDate && x.Dt <= reportEndDate
                )
                .GroupBy(
                    x => x.Dt.WeekOfMonth()
                )
                .Select(
                    x => new WeeklyReportBuilder(x).Build()
                );
            return new ForexMonthlyReport(
                reportStartDate,
                weeklyReports
            );*/

            var reportStartDate = new DateTime(Year, Month, 1);
            var reportEndDate = reportStartDate.AddDays(DateTime.DaysInMonth(Year, Month) - 1);
            var weeklyReports = Enumerable.Range(1, DateTimeExtensions.WeeksInMonth(Year, Month))
                .Select(
                    x => new DateTime(Year, Month, DateTimeExtensions.FirstDayOfWeek(Year, Month, x))
                )
                .Select(
                    week => WeeklyBuilder
                        .SetDate(week)
                        .SetBaseCurrency(BaseCurrency)
                        .SetCounterCurrency(CounterCurrency)
                        .SetData(
                            Pairs.Where(
                                pair => pair.Dt >= week && pair.Dt <= DateTimeExtensions.LastDtOfWeek(week)
                            )
                            .ToArray()
                        )
                        .Build()
                )
                .ToArray();

            return new ForexMonthlyReport(
                reportStartDate,
                weeklyReports
            );
        }
    }
}
