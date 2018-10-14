using System;
using System.Collections.Generic;

namespace DomainModels
{
    public class ForexMonthlyReport
    {
        public DateTime Dt { get; private set; }
        public IEnumerable<ForexWeeklyReport> WeeklyReports { get; private set; }

        public ForexMonthlyReport(DateTime dt, IEnumerable<ForexWeeklyReport> weeklyReports)
            => (Dt, WeeklyReports) = (dt, weeklyReports);
    }
}
