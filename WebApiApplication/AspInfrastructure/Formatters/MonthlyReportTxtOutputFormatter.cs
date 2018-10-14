using DomainModels;
using Infrastructure.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiApplication.AspInfrastructure.Formatters
{
    public class MonthlyReportTxtOutputFormatter : TextOutputFormatter
    {
        public MonthlyReportTxtOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/plain"));

            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        protected override bool CanWriteType(Type type)
        {
            return type.Equals(
                typeof(Task<IEnumerable<ForexMonthlyReport>>)
            );
        }

        public override Task WriteResponseBodyAsync(
            OutputFormatterWriteContext context, Encoding selectedEncoding
        )
        {
            /*IServiceProvider serviceProvider = context.HttpContext.RequestServices;
            var logger = serviceProvider.GetService(typeof(ILogger<VcardOutputFormatter>)) as ILogger;
            */
            var response = context.HttpContext.Response;

            var reports = context.Object as IEnumerable<ForexMonthlyReport>;

            int reportYear = reports.First().Dt.Year;
            int reportMonth = reports.First().Dt.Month;
            var buf = new StringBuilder();
            buf.AppendLine(
                $"Year {reportYear}, month: {reports.First().Dt.ToString("MMMM", CultureInfo.InvariantCulture)}"
            );

            for (
                int week = 1;
                week <= DateTimeExtensions.WeeksInMonth(reportYear, reportMonth);
                ++week
            )
            {
                buf.Append(DateTimeExtensions.FirstDayOfWeek(reportYear, reportMonth, week));
                buf.Append("..");
                buf.Append(DateTimeExtensions.LastBusinessDayOfWeek(reportYear, reportMonth, week));
                buf.Append(": ");

                var rates = reports
                    .Select(
                        x => x.WeeklyReports.ElementAt(week - 1)
                    )
                    .Select(r =>
                        $"{r.BaseSymbol} - max: {r.MaxRate}, min: {r.MinRate}, median: {r.MedianRate};"
                    );

                buf.AppendLine(
                    string.Join(" ", rates.ToArray())
                );
            }
            
            return response.WriteAsync(buf.ToString());
        }
    }
}
