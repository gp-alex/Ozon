using System;

namespace DomainModels
{
    public class ForexWeeklyReport
    {
        public DateTime ReportStartDt { get; private set; }
        public DateTime ReportEndDt { get; private set; }
        public string BaseSymbol { get; private set; }
        public string CounterSymbol { get; private set; }
        public double MinRate { get; private set; }
        public double MaxRate { get; private set; }
        public double MedianRate { get; private set; }

        public ForexWeeklyReport(
            DateTime reportStartDt,
            DateTime reportEnDt,
            string baseSymbol,
            string counterSymbol,
            double minRate,
            double maxRate,
            double medianRate
        )
            => (ReportStartDt, ReportEndDt, BaseSymbol, CounterSymbol, MinRate, MaxRate, MedianRate)
            = (reportStartDt, reportEnDt, baseSymbol, counterSymbol, minRate, maxRate, medianRate);
    }
}
