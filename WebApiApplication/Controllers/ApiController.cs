using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainModels;
using DomainModels.Filters;
using DomainServices;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using WebApiApplication.AspInfrastructure;

namespace WebApiApplication.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly IEnumerable<string> RequiredSymbols;
        private readonly IPairRepository pairRepo;
        private readonly MonthlyReportBuilder monthlyReportBuilder;
        private readonly ILogger log;
        public ApiController(
            IOptions<ApiControlerOptions> options,
            IPairRepository pairRepo,
            MonthlyReportBuilder monthlyReportBuilder,
            ILogger log
        )
        {
            this.RequiredSymbols = options?.Value?.RequiredSymbols ?? throw new ArgumentNullException(nameof(RequiredSymbols));
            this.pairRepo = pairRepo ?? throw new ArgumentNullException(nameof(pairRepo));
            this.monthlyReportBuilder = monthlyReportBuilder ?? throw new ArgumentNullException(nameof(monthlyReportBuilder));
            this.log = log?.ForContext(GetType());
        }

        [HttpGet("report/{year}/{month}.{format?}"), FormatFilter]
        public async Task<IEnumerable<ForexMonthlyReport>> Report(int year, int month)
        {
            var tasks = RequiredSymbols.Select(
                async symbol => await pairRepo.FindAsync(
                    new PairDateFilter(month, year)
                        .And(new PairBaseSymbolFilter(symbol))
                )
            );

            var reports = (await Task.WhenAll(tasks))
                .Select(
                    data => monthlyReportBuilder
                        .SetYear(year)
                        .SetMonth(month)
                        .SetBaseCurrency(data.First().BaseSymbol)
                        .SetCounterCurrency(data.First().CounterSymbol)
                        .SetData(data)
                        .Build()
                );

            return reports;
        }
    }
}
